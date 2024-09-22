using ListSky.Lib.DTO;

namespace ListSky.Lib.Import;

public class ExternalSourceReport
{
    public ListMetadata ListMetadata { get; set; } = null!;
    public IEnumerable<ListEntry> Added { get; set; } = null!;
    public IEnumerable<ListEntry> Updated { get; set; } = null!;
    public IEnumerable<ListEntry> Removed { get; set; } = null!;
}