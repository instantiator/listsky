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
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine($"Target path is blank");
                return false;
            }

            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            var listsPath = Path.Combine(targetPath, "lists");
            if (!Directory.Exists(listsPath)) Directory.CreateDirectory(listsPath);

            var config = Config.FromEnv();
            var files = DocsGenerator.Render(config);

            foreach (var file in files)
            {
                File.WriteAllText(Path.Combine(targetPath, file.Path), file.Html);
            }

            Console.WriteLine(JsonSerializer.Serialize(files, new JsonSerializerOptions { WriteIndented = true }));
            return true;
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
