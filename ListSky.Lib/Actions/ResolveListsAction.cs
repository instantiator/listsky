
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;

namespace ListSky.Lib.Actions;

public class ResolveListsAction : AbstractAction
{
    public ResolveListsAction(Config config) : base(config)
    {
    }

    protected override async Task ExecuteImplementationAsync(ActionResult result)
    {
        var allFoundLists = await connection.GetListsAsync();
        foreach (var listData in config.AllListData.Lists)
        {
            result.Outputs.Add($"List: {listData.Title}, {listData.Path_CSV}, {listData.ListId}");
            var listEntries = config.ReadList(listData.Path_CSV);
            var foundList = allFoundLists.FirstOrDefault(l => l.Uri.Pathname.EndsWith($"/{listData.ListId}"));
            if (foundList == null) throw new Exception($"List not found: {listData.ListId}");
            var foundListItems = await connection.GetListItemsAsync(foundList.Uri);
            var actions = ListManager.Compare(listEntries, foundListItems);

            foreach (var entry in actions.ToDelete)
            {
                result.Outputs.Add($"Removing: {entry.Subject.DisplayName}, {entry.Uri}");
                await connection.RemovePersonFromListAsync(foundList.Uri, entry.Uri.Did!);
            }

            foreach (var entry in actions.ToAdd)
            {
                result.Outputs.Add($"Adding: {entry.Name}, {entry.AccountName_BlueSky}");
                var subject = await connection.FindPersonByHandleAsync(entry.AccountName_BlueSky);
                if (subject == null) throw new Exception($"Subject not found: {entry.AccountName_BlueSky}");

                await connection.AddPersonToListAsync(foundList.Uri, subject!.Did!);
            }
        }
    }
}