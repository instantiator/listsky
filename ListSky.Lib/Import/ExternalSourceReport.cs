using ListSky.Lib.DTO;

namespace ListSky.Lib.Import;

public class ExternalSourceReport
{
    public ListMetadata ListMetadata { get; set; } = null!;
    public IEnumerable<ListEntry> Add { get; set; } = null!;
    public IEnumerable<ListEntry> Update { get; set; } = null!;
    public IEnumerable<ListEntry> Remove { get; set; } = null!;
}