using ListSky.Lib.DTO;

namespace ListSky.Lib.Templating;

public class ListModel
{
    public ListMetadata Metadata { get; set; } = null!;
    public IEnumerable<ListEntry> Entries { get; set; } = null!;
    
}