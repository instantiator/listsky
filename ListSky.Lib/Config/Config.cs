using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace ListSky.Lib.DTO;

public class Config
{
    public string Server_AT { get; set; } = null!;
    public string AccountName_AT { get; set; } = null!;
    public string AppPassword_AT { get; set; } = null!;
    public string Path_AllListsMetadataJson { get; set; } = null!;

    public AllLists AllListData => JsonSerializer.Deserialize<AllLists>(File.ReadAllText(Path_AllListsMetadataJson))!;

    public IEnumerable<ListEntry> ReadList(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            AllowComments = true,
            Delimiter = ",",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            // PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
            return csv.GetRecords<ListEntry>().ToList();
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
        };
    }
}