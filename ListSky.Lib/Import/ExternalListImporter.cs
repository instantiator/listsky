using ListSky.Lib.Config;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Import;

public enum ExternalListFormat { CSV, JSON }

public class ExternalListImporter(ListMetadata metadata, string providence, ExternalListFormat format)
{

    public async Task<ExternalSourceReport> ImportAsync()
    {
        var uri = new Uri(providence);

        // fetch entries from the specified url
        var entries = format switch
        {
            ExternalListFormat.CSV => await CsvListIO.ReadUriAsync(uri),
            ExternalListFormat.JSON => await JsonListIO.ReadUriAsync(uri),
            _ => throw new NotImplementedException($"Unsupported format: {format}")
        };

        // set providence on all retrieved entries
        entries = entries.Select(e => { e.Providence = providence; return e; });

        // retrieve all current entries for this source
        var allEntries = CsvListIO.ReadFile(metadata.Path_CSV);

        var report = DiffLists(metadata, providence, allEntries, entries);

        // write the new entries to the target list
        var newEntries = ApplyChanges(allEntries, report.Add, report.Update, report.Remove);
        CsvListIO.WriteFile(metadata.Path_CSV, newEntries);

        return report;
    }

    public static ExternalSourceReport DiffLists(ListMetadata metadata, string providence, IEnumerable<ListEntry> allEntries, IEnumerable<ListEntry> newEntries)
    {
        var relevantCurrentEntries = allEntries.Where(e => e.Providence == providence);
        var relevantEntries_add = newEntries.Where(e => !relevantCurrentEntries.Any(re => re.IsProbably(e)));
        var relevantEntries_update = newEntries.Where(e => relevantCurrentEntries.Any(re => re.IsProbably(e)));
        var relevantEntries_remove = relevantCurrentEntries.Where(re => !newEntries.Any(e => e.IsProbably(re)));

        return new ExternalSourceReport
        {
            ListMetadata = metadata,
            Add = relevantEntries_add,
            Update = relevantEntries_update,
            Remove = relevantEntries_remove
        };
    }

    private IEnumerable<ListEntry> ApplyChanges(IEnumerable<ListEntry> currentEntries, IEnumerable<ListEntry> add, IEnumerable<ListEntry> update, IEnumerable<ListEntry> remove)
    {
        IEnumerable<ListEntry> modifiedList = new List<ListEntry>(currentEntries);
        modifiedList = modifiedList.Where(e => !remove.Any(r => r.IsProbably(e))).ToList();
        modifiedList = modifiedList.Select(e => update.FirstOrDefault(u => u.IsProbably(e))?.UpdateInto(e) ?? e).ToList();
        modifiedList = modifiedList.Concat(add);
        return modifiedList;
    }
}
