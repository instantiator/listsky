using ListSky.Lib.IO;

namespace ListSky.Lib.Actions;

public class ReportOnExternalSourcesAction : AbstractAction<IEnumerable<ExternalSourceReport>>
{
    public ReportOnExternalSourcesAction(Config.Config config) : base(config)
    {
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<ExternalSourceReport>> result)
    {
        var reports = new List<ExternalSourceReport>();
        foreach (var list in config.AllListData.Lists)
        {
            result.Outputs.Add($"Examining: {list.Slug}");
            foreach (var source in list.ExternalSources_CSV ?? Enumerable.Empty<string>())
            {
                result.Outputs.Add($"External CSV source: {source}");
                var importer = new ExternalListImporter(list, source, ExternalListFormat.CSV);
                var report = await importer.ImportAsync();
                if (report.ContainsChanges) { reports.Add(report); }
            }

            foreach (var source in list.ExternalSources_JSON ?? Enumerable.Empty<string>())
            {
                result.Outputs.Add($"External JSON source: {source}");
                var importer = new ExternalListImporter(list, source, ExternalListFormat.JSON);
                var report = await importer.ImportAsync();
                if (report.ContainsChanges) { reports.Add(report); }
            }
        }

        result.Data = reports;
        return true;
    }
}