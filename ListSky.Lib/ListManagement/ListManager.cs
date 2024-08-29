using FishyFlip.Models;
using ListSky.Lib.DTO;

namespace ListSky.Lib.ListManagement;
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

        foreach (var entry in authoritativeList)
        {
            if (!foundList.Any(l => l.Subject.Handle == entry.AccountName_BlueSky))
            {
                toAdd.Add(entry);
            }
        }

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