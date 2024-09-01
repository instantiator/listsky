using System.Text.Json;
using ListSky.Lib.Actions;
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;
using ListSky.Lib.Templating;

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
                var applyResult = await ApplyListsAsync();
                return applyResult ? 0 : 1;

            case "document":
                var target = args[1];
                var docResult = await DocumentListsAsync(target);
                return docResult ? 0 : 1;

            default:
                Console.WriteLine("Unknown command: " + args[0]);
                PrintArgs();
                return 1;
        }
    }

    private static async Task<bool> DocumentListsAsync(string targetPath)
    {
        try
        {
            var config = Config.FromEnv();
            var action = new DocumentListsAction(config, targetPath);
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


    private static async Task<bool> ApplyListsAsync()
    {
        try
        {
            var config = Config.FromEnv();
            var action1 = new ResolveListsAction(config);
            var result1 = await action1.ExecuteAsync();

            // even if not a full success, some actions may have succeeded - these should be posted about
            if (result1.Data != null)
            {
                var action2 = new PostChangesAction(config, result1.Data);
                var result2 = await action2.ExecuteAsync();
                Console.WriteLine(JsonSerializer.Serialize(new object[] { result1, result2 }, new JsonSerializerOptions { WriteIndented = true }));
                return result1.Success && result2.Success;
            }
            else
            {
                Console.WriteLine(JsonSerializer.Serialize(result1, new JsonSerializerOptions { WriteIndented = true }));
                return result1.Success;
            }

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
        Console.WriteLine("  apply    - modify lists in BlueSky to match authoritative CSV lists");
        Console.WriteLine("  document - generate documentation pages for all lists");
    }
}
