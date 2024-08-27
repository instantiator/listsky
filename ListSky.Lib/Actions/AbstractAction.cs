using System.Text.Json;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Actions;

public abstract class AbstractAction
{
    protected Config config;

    public AbstractAction(Config config)
    {
        this.config = config;
    }

    public abstract Task ExecuteAsync();
}
