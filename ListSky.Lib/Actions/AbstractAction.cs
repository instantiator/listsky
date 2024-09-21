using FishyFlip.Models;
using ListSky.Lib.BlueSky.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Actions;

public abstract class AbstractAction<DataType>
{
    protected Config config;
    protected ATConnection connection;
    protected Session? session;

    public AbstractAction(Config config)
    {
        this.config = config;
        this.connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
    }

    public async Task<ActionResult<DataType>> ExecuteAsync()
    {
        ActionResult<DataType> result = new()
        {
            Started = DateTime.Now
        };

        try
        {
            session = await connection.ConnectAsync();
            result.Success = await ExecuteImplementationAsync(result);
        }
        catch (Exception e)
        {
            result.Exception = e;
            result.Success = false;
        }
        finally
        {
            result.Finished = DateTime.Now;
            connection.Disconnect();
            session = null;
        }
        return result;
    }

    protected abstract Task<bool> ExecuteImplementationAsync(ActionResult<DataType> result);
}
