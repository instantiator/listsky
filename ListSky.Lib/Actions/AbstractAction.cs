using System.Text.Json;
using FishyFlip.Models;
using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Actions;

public abstract class AbstractAction
{
    protected Config config;
    protected ATConnection connection;
    protected Session? session;

    public AbstractAction(Config config)
    {
        this.config = config;
        this.connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
    }

    public async Task ExecuteAsync()
    {
        this.session = await connection.ConnectAsync();
        await ExecuteImplementationAsync();
        connection.Disconnect();
    }

    protected abstract Task ExecuteImplementationAsync();
}
