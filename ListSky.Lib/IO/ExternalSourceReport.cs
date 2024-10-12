using ListSky.Lib.DTO;

namespace ListSky.Lib.IO;

public class ExternalSourceReport
{
    public ListMetadata ListMetadata { get; set; } = null!;
    public IEnumerable<ListEntry> Add { get; set; } = new List<ListEntry>();
    public IEnumerable<ListEntry> Update { get; set; } = new List<ListEntry>();
    public IEnumerable<ListEntry> Remove { get; set; } = new List<ListEntry>();
    public bool ContainsChanges => Add.Any() || Update.Any() || Remove.Any();
}