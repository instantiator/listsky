
using ListSky.Lib.DTO;
using ListSky.Lib.Templating;

namespace ListSky.Lib.Actions;

public class DocumentListsAction : AbstractAction<IEnumerable<DocFile>>
{
    protected string targetPath;

    public DocumentListsAction(Config config, string targetPath) : base(config)
    {
        this.targetPath = targetPath;
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<DocFile>> result)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
        {
            throw new Exception("Target path is blank");
        }

        if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

        var listsPath = Path.Combine(targetPath, "lists");
        if (!Directory.Exists(listsPath)) Directory.CreateDirectory(listsPath);

        var config = Config.FromEnv();
        var files = DocsGenerator.Render(config);

        foreach (var file in files)
        {
            await File.WriteAllTextAsync(Path.Combine(targetPath, file.Path), file.Html);
            result.Outputs.Add($"Written: {file.Path}");
        }

        result.Data = files;
        return true;
    }
}

