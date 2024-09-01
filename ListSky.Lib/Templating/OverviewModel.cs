using ListSky.Lib.DTO;

namespace ListSky.Lib.Templating;

public class OverviewModel
{
    public string Server_AT { get; set; } = null!;
    public string AccountName_AT { get; set; } = null!;
    public string GitHub_Repo { get; set; } = null!;
    public string GitHub_User { get; set; } = null!;

    public AllLists AllLists { get; set; } = null!;
}