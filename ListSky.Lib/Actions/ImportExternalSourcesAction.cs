using ListSky.Lib.DTO;
using ListSky.Lib.Import;

namespace ListSky.Lib.Actions;

public class ImportExternalSourcesAction : AbstractAction<IEnumerable<ExternalSourceReport>>
{
    public ImportExternalSourcesAction(Config.Config config) : base(config)
    {
    }

    protected override Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<ExternalSourceReport>> result)
    {
        throw new NotImplementedException();
    }
}