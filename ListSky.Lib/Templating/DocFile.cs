using System.Text.Json.Serialization;

namespace ListSky.Lib.Templating;

public class DocFile
{
    public string Path { get; set; } = null!;
    [JsonIgnore] public string Html { get; set; } = null!;
}
