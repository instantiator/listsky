using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ListSky.Lib.DTO;

namespace ListSky.Lib.IO;

public static class CsvListIO
{
    private static CsvConfiguration CSV_CONFIG = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        AllowComments = true,
        Delimiter = ",",
        HasHeaderRecord = true,
        IgnoreBlankLines = true,
        // PrepareHeaderForMatch = args => args.Header.ToLower(),
    };

    public static IEnumerable<ListEntry> ReadFile(string path)
    {
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CSV_CONFIG))
        {
            return csv.GetRecords<ListEntry>().ToList();
        }
    }

    public static void WriteFile(string path, IEnumerable<ListEntry> list)
    {
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CSV_CONFIG))
        {
            csv.WriteRecords(list);
        }
    }

    public static string GenerateFileContent(IEnumerable<ListEntry> list)
    {
        using (var writer = new StringWriter())
        using (var csv = new CsvWriter(writer, CSV_CONFIG))
        {
            csv.WriteRecords(list);
            return writer.ToString();
        }
    }

    public static async Task<IEnumerable<ListEntry>> ReadUriAsync(Uri uri)
    {
        using (var client = new HttpClient())
        using (var stream = await client.GetStreamAsync(uri))
        using (var reader = new StreamReader(stream))
        using (var csv = new CsvReader(reader, CSV_CONFIG))
        {
            return csv.GetRecords<ListEntry>().ToList();
        }
    }
}