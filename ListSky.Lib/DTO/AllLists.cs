namespace ListSky.Lib.DTO;

public class AllLists
{
    public string Title { get; set; } = null!;
    public string SubTitle { get; set; } = null!;
    public IEnumerable<string> Descriptions { get; set; } = null!;
    public IEnumerable<ListMetadata> Lists { get; set; } = new List<ListMetadata>();
}