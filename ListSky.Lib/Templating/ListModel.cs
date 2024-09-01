using ListSky.Lib.DTO;

namespace ListSky.Lib.Templating;

public class ListModel
{
    public string Server_AT { get; set; } = null!;
    public string AccountName_AT { get; set; } = null!;
    public string GitHub_Repo { get; set; } = null!;
    public string GitHub_User { get; set; } = null!;

    public ListMetadata Metadata { get; set; } = null!;
    public IEnumerable<ListEntry> Entries { get; set; } = null!;

}