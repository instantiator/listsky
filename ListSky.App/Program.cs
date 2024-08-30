using System.Text.Json;
using ListSky.Lib.Actions;
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;

namespace ListSky.App;

public class ListSkyApp
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintArgs(); return 1;
        }

        switch (args[0].ToLower().Trim())
        {
            case "apply":
                var result = await ApplyListsAsync();
                return result ? 0 : 1;
            default:
                Console.WriteLine("Unknown command: " + args[0]);
                PrintArgs();
                return 1;
        }
    }

    private static async Task<bool> ApplyListsAsync()
    {
        try
        {
            var config = Config.FromEnv();
            var action = new ResolveListsAction(config);
            var result = await action.ExecuteAsync();
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
            return result.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static void PrintArgs()
    {
        Console.WriteLine("Arguments: <command> [options]");
        Console.WriteLine("Commands:");
        Console.WriteLine("  update - modify lists in BlueSky to match CSV provided");
    }
}
