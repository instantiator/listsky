using ListSky.Lib.DTO;

namespace ListSky.Lib.IO;

public class ExternalSourceReportUtils
{
    public static Dictionary<string, ExternalSourceReport> CombineReports(IEnumerable<ExternalSourceReport> reports) =>
        reports
            .GroupBy(r => r.ListMetadata.Slug)
            .Select(g => new KeyValuePair<string, ExternalSourceReport>(g.Key, CombineReportsForList(g.Key, g)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public static ExternalSourceReport CombineReportsForList(string slug, IEnumerable<ExternalSourceReport> reports)
    {
        if (!reports.All(r => r.ListMetadata.Slug == slug))
        {
            throw new ArgumentException($"All reports must refer to slug: {slug}");
        }

        var anyMetadata = reports.First().ListMetadata;
        var combinedAdd = reports.SelectMany(r => r.Add);
        var combinedUpdate = reports.SelectMany(r => r.Update);
        var combinedRemove = reports.SelectMany(r => r.Remove);

        return new ExternalSourceReport
        {
            ListMetadata = anyMetadata,
            Add = combinedAdd,
            Update = combinedUpdate,
            Remove = combinedRemove,
        };
    }

    public static ExternalSourceReport DiffLists(ListMetadata metadata, string providenceUri, IEnumerable<ListEntry> allEntries, IEnumerable<ListEntry> newEntries)
    {
        var relevantCurrentEntries = allEntries.Where(e => e.Providence == providenceUri);
        var relevantEntries_add = newEntries.Where(e => !relevantCurrentEntries.Any(re => re.IsProbably(e)));
        var relevantEntries_update = newEntries.Where(e => relevantCurrentEntries.Any(re => re.IsProbably(e) && re.DiffersTo(e)));
        var relevantEntries_remove = relevantCurrentEntries.Where(re => !newEntries.Any(e => e.IsProbably(re)));

        return new ExternalSourceReport
        {
            ListMetadata = metadata,
            Add = relevantEntries_add,
            Update = relevantEntries_update,
            Remove = relevantEntries_remove,
        };
    }

    public static IEnumerable<ListEntry> ApplyChanges(IEnumerable<ListEntry> currentEntries, ExternalSourceReport report)
    {
        IEnumerable<ListEntry> modifiedList = new List<ListEntry>(currentEntries);

        // remove entries indicated in the remove list
        modifiedList = modifiedList.Where(e => !report.Remove.Any(r => r.IsProbably(e))).ToList();

        // select updated entries where found in the update list, otherwise keep the original entry
        modifiedList = modifiedList.Select(e => report.Update.FirstOrDefault(u => u.IsProbably(e))?.UpdateInto(e) ?? e);

        // concatenate the add list to the end
        modifiedList = modifiedList.Concat(report.Add);

        return modifiedList;
    }


}