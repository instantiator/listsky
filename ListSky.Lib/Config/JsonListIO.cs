using System.Text.Json;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Config;

public class JsonListIO
{

    public static async Task<IEnumerable<ListEntry>> ReadUriAsync(Uri uri)
    {
        using (var client = new HttpClient())
        {
            var json = await client.GetStringAsync(uri);
            return JsonSerializer.Deserialize<IEnumerable<ListEntry>>(json)!;
        }
    }
}