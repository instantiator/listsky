namespace ListSky.Lib;

public class ListMetadata
{
    public string ListId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Path_CSV { get; set; } = null!;
    public bool Publish { get; set; }
}