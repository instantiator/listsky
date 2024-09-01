
using FishyFlip.Models;
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;

namespace ListSky.Lib.Actions;

public class ListResolutions : List<KeyValuePair<ListMetadata, ListManager.ListManagerActions>>
{
}

public class ResolveListsAction : AbstractAction<ListResolutions>
{
    public ResolveListsAction(Config config) : base(config)
    {
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<ListResolutions> result)
    {
        var allFoundLists = await connection.GetListsAsync();
        var succeeded = true;
        var exceptions = new List<Exception>();
        var resolution = new ListResolutions();

        foreach (var listData in config.AllListData.Lists)
        {
            var successfulAdditions = new List<ListEntry>();
            var successfulRemovals = new List<ListItemView>();

            result.Outputs.Add($"List: {listData.Title}, {listData.Path_CSV}, {listData.ListId}");
            var listEntries = config.ReadList(listData.Path_CSV);
            var foundList = allFoundLists.FirstOrDefault(l => l.Uri.Pathname.EndsWith($"/{listData.ListId}"));
            if (foundList == null) throw new Exception($"List not found: {listData.ListId}");
            var foundListItems = await connection.GetListItemsAsync(foundList.Uri);
            var actions = ListManager.Compare(listEntries, foundListItems);


            foreach (var entry in actions.ToDelete)
            {
                try
                {
                    result.Outputs.Add($"Removing: {entry.Subject.DisplayName}, {entry.Uri}");
                    await connection.RemovePersonFromListAsync(foundList.Uri, entry.Uri.Did!);
                    successfulRemovals.Add(entry);
                }
                catch (Exception e)
                {
                    succeeded = false;
                    exceptions.Add(e);
                }
            }

            foreach (var entry in actions.ToAdd)
            {
                try
                {
                    result.Outputs.Add($"Adding: {entry.Name}, {entry.AccountName_BlueSky}");
                    var subject = await connection.FindPersonByHandleAsync(entry.AccountName_BlueSky);
                    if (subject == null) throw new Exception($"Subject not found: {entry.AccountName_BlueSky}");
                    await connection.AddPersonToListAsync(foundList.Uri, subject!.Did!);
                    successfulAdditions.Add(entry);
                }
                catch (Exception e)
                {
                    succeeded = false;
                    exceptions.Add(e);
                }
            }

            result.Outputs.Add($"{listData.Slug}: {successfulAdditions.Count} successful additions");
            result.Outputs.Add($"{listData.Slug}: {successfulRemovals.Count} successful removals");

            resolution.Add(new KeyValuePair<ListMetadata, ListManager.ListManagerActions>(
                listData,
                new ListManager.ListManagerActions()
                {
                    ToDelete = successfulRemovals,
                    ToAdd = successfulAdditions
                }));
        }

        result.Data = resolution;
        result.Exception = exceptions.Count() == 0 ? null : new AggregateException("Unable to resolve all list actions", exceptions);
        return succeeded;
    }
}