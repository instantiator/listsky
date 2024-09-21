using FishyFlip.Models;
using ListSky.Lib.DTO;

namespace ListSky.Lib.BlueSky.ListManagement;
public class ListManager
{

    public class ListManagerActions
    {
        public IEnumerable<ListItemView> ToDelete { get; set; } = new List<ListItemView>();
        public IEnumerable<ListEntry> ToAdd { get; set; } = new List<ListEntry>();
    }

    public static ListManagerActions Compare(IEnumerable<ListEntry> authoritativeList, IEnumerable<ListItemView> foundList)
    {
        var toDelete = new List<ListItemView>();
        var toAdd = new List<ListEntry>();

        // only add list members that have a BlueSky account name
        foreach (var entry in authoritativeList.Where(entry => !string.IsNullOrWhiteSpace(entry.AccountName_BlueSky)))
        {
            if (!foundList.Any(l => l.Subject.Handle == entry.AccountName_BlueSky))
            {
                toAdd.Add(entry);
            }
        }

        // remove list members that don't have an account listed
        foreach (var entry in foundList)
        {
            if (!authoritativeList.Any(l => l.AccountName_BlueSky == entry.Subject.Handle))
            {
                toDelete.Add(entry);
            }
        }

        return new ListManagerActions
        {
            ToDelete = toDelete,
            ToAdd = toAdd
        };

    }
}