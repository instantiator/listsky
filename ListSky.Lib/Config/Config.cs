using System.Text.Json;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Config;

public class Config
{
    public string Server_AT { get; set; } = null!;
    public string AccountName_AT { get; set; } = null!;
    public string AppPassword_AT { get; set; } = null!;
    public string Path_AllListsMetadataJson { get; set; } = null!;
    public string GITHUB_REPO { get; set; } = null!;
    public string GITHUB_USER { get; set; } = null!;
    public string? GITHUB_TOKEN { get; set; }

    private static readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    private AllLists? _allLists = null;
    public AllLists AllListData
    {
        get
        {
            _allLists = _allLists ?? JsonSerializer.Deserialize<AllLists>(File.ReadAllText(Path_AllListsMetadataJson), options)!;
            return _allLists;
        }
    }

    public static Config FromEnv()
    {
        return new Config()
        {
            Server_AT = Environment.GetEnvironmentVariable("Server_AT") ?? throw new Exception("Server_AT not set"),
            AccountName_AT = Environment.GetEnvironmentVariable("AccountName_AT") ?? throw new Exception("AccountName_BlueSky not set"),
            AppPassword_AT = Environment.GetEnvironmentVariable("AppPassword_AT") ?? throw new Exception("AppPassword_BlueSky not set"),
            Path_AllListsMetadataJson = Environment.GetEnvironmentVariable("Path_AllListsMetadataJson") ?? throw new Exception("Path_AllListsMetadataJson not set"),
            GITHUB_REPO = Environment.GetEnvironmentVariable("GITHUB_REPO")!.Split('/').Last() ?? throw new Exception("GITHUB_REPO not set"),
            GITHUB_USER = Environment.GetEnvironmentVariable("GITHUB_USER") ?? throw new Exception("GITHUB_USER not set"),
            GITHUB_TOKEN = Environment.GetEnvironmentVariable("GITHUB_TOKEN") // may not be present
        };
    }
}