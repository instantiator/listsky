using ListSky.Lib.Config;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Import;

public enum ExternalListFormat { CSV, JSON }

public class ExternalListImporter(ListMetadata target, string url, ExternalListFormat format)
{

    public async Task<ExternalSourceReport> ImportAsync()
    {
        var uri = new Uri(url);

        // fetch entries from the specified url
        var entries = format switch
        {
            ExternalListFormat.CSV => await CsvListIO.ReadUriAsync(uri),
            ExternalListFormat.JSON => await JsonListIO.ReadUriAsync(uri),
            _ => throw new NotImplementedException($"Unsupported format: {format}")
        };

        // set providence on all retrieved entries
        entries = entries.Select(e => { e.Providence = url; return e; });

        // retrieve all current entries for this source
        var currentEntries = CsvListIO.ReadFile(target.Path_CSV).Where(e => e.Providence == url);

        // diff the current entries with the new entries
        var currentEntries_add = entries.Where(e => !currentEntries.Any(ce => ce.IsProbably(e)));
        var currentEntries_update = entries.Where(e => currentEntries.Any(ce => ce.IsProbably(e)));
        var currentEntries_remove = currentEntries.Where(ce => !entries.Any(e => e.IsProbably(ce)));

        var newEntries = ApplyChanges(currentEntries, currentEntries_add, currentEntries_update, currentEntries_remove);

        // write the new entries to the target list
        CsvListIO.WriteFile(target.Path_CSV, newEntries);

        return new ExternalSourceReport
        {
            ListMetadata = target,
            Added = currentEntries_add,
            Updated = currentEntries_update,
            Removed = currentEntries_remove
        };
    }

    private IEnumerable<ListEntry> ApplyChanges(IEnumerable<ListEntry> currentEntries, IEnumerable<ListEntry> add, IEnumerable<ListEntry> update, IEnumerable<ListEntry> remove)
    {
        var newEntries = currentEntries.ToList();
        newEntries = newEntries.Where(e => !remove.Any(r => r.IsProbably(e))).ToList();
        newEntries = newEntries.Select(e => update.FirstOrDefault(u => u.IsProbably(e)) ?? e).ToList();
        newEntries.AddRange(add);
        return newEntries;
    }
}
 