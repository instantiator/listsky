using ListSky.Lib.DTO;
using ListSky.Lib.Import;

namespace ListSky.Lib.Actions;

public class ImportExternalSourcesAction : AbstractAction<IEnumerable<ExternalSourceReport>>
{
    public ImportExternalSourcesAction(Config.Config config) : base(config)
    {
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<ExternalSourceReport>> result)
    {
        var reports = new List<ExternalSourceReport>();

        foreach (var list in config.AllListData.Lists)
        {
            foreach (var source in list.ExternalSources_CSV ?? Enumerable.Empty<string>())
            {
                var importer = new ExternalListImporter(list, source, ExternalListFormat.CSV);
                var report = await importer.ImportAsync();
                reports.Add(report);
            }

            foreach (var source in list.ExternalSources_JSON ?? Enumerable.Empty<string>())
            {
                var importer = new ExternalListImporter(list, source, ExternalListFormat.JSON);
                var report = await importer.ImportAsync();
                reports.Add(report);
            }
        }

        result.Data = reports;
        return true;
    }
}