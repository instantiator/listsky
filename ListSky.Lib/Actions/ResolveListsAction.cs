
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;

namespace ListSky.Lib.Actions;

public class ResolveListsAction : AbstractAction
{
    public ResolveListsAction(Config config) : base(config)
    {
    }

    protected override async Task ExecuteImplementationAsync()
    {
        var allFoundLists = await connection.GetListsAsync();
        foreach (var listData in config.AllListData.Lists)
        {
            var listEntries = config.ReadList(listData.Path_CSV);
            var foundList = allFoundLists.FirstOrDefault(l => l.Uri.Pathname.EndsWith($"/{listData.ListId}"));
            if (foundList == null) throw new Exception("List not found: ${listData.ListId}");
            var foundListItems = await connection.GetListItemsAsync(foundList.Uri);
            var actions = ListManager.Compare(listEntries, foundListItems);

            foreach (var entry in actions.ToDelete)
            {
                await connection.RemovePersonFromListAsync(foundList.Uri, entry.Uri.Did!);
            }

            foreach (var entry in actions.ToAdd)
            {
                var subject = await connection.FindPersonByHandleAsync(entry.AccountName_BlueSky);
                if (subject == null) throw new Exception($"Subject not found: {entry.AccountName_BlueSky}");

                await connection.AddPersonToListAsync(foundList.Uri, subject!.Did!);
            }
        }


    }
}