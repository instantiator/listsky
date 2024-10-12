using ListSky.Lib.DTO;

namespace ListSky.Lib.IO;

public enum ExternalListFormat { CSV, JSON }

public class ExternalListImporter(ListMetadata metadata, string providenceUri, ExternalListFormat format)
{

    public async Task<ExternalSourceReport> ImportAsync()
    {
        var uri = new Uri(providenceUri);

        // fetch entries from the specified url
        var importEntries = format switch
        {
            ExternalListFormat.CSV => await CsvListIO.ReadUriAsync(uri),
            ExternalListFormat.JSON => await JsonListIO.ReadUriAsync(uri),
            _ => throw new NotImplementedException($"Unsupported format: {format}")
        };

        // set providence on all retrieved entries
        importEntries = importEntries.Select(e => { e.Providence = providenceUri; return e; });

        // retrieve all current entries for this source
        var currentEntries = CsvListIO.ReadFile(metadata.Path_CSV);

        // diff the new list
        return ExternalSourceReportUtils.DiffLists(metadata, providenceUri, currentEntries, importEntries);
    }

}
