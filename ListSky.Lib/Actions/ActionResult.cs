using System.Text.Json.Serialization;

namespace ListSky.Lib.Actions;

public class ActionResult
{
    public DateTime Started { get; set; }
    public DateTime Finished { get; set; }
    public TimeSpan Duration => Finished - Started;
    public List<string> Outputs { get; set; } = new List<string>();
    public bool Success { get; set; }
    [JsonIgnore] public Exception? Exception { get; set; }
    public string? ExceptionString => Exception?.ToString();
}