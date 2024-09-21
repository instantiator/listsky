
using FishyFlip.Models;
using ListSky.Lib.BlueSky.Composition;
using ListSky.Lib.DTO;

namespace ListSky.Lib.Actions;

public class PostSummary
{
    public string Message { get; set; } = null!;
    public CreatePostResponse? Response { get; set; }
    public bool Succeeded { get; set; }
    public Exception? Exception { get; set; }
}

public class PostChangesAction : AbstractAction<IEnumerable<PostSummary>>
{
    protected ListResolutions resolutions;
    protected MessageComposer composer;

    public PostChangesAction(Config config, ListResolutions resolutions) : base(config)
    {
        this.resolutions = resolutions;
        this.composer = new MessageComposer(connection);
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<PostSummary>> result)
    {
        var results = new List<PostSummary>();

        foreach (var listResolution in resolutions)
        {
            foreach (var addition in listResolution.Value.ToAdd)
            {
                var values = new Dictionary<string, string>()
                {
                    { "account", addition.AccountName_BlueSky },
                    { "listName", listResolution.Key.Title },
                    { "listUrl", $"https://bsky.app/profile/{config.AccountName_AT}/lists/{listResolution.Key.ListId}" },
                };
                Tuple<string, IEnumerable<Facet>> data = await composer.ComposeAsync(Composition.AddedPersonToList, values);
                try
                {
                    var response = await connection.PostAsync(data.Item1, data.Item2);
                    results.Add(new PostSummary
                    {
                        Message = data.Item1,
                        Response = response,
                        Succeeded = response != null
                    });
                }
                catch (Exception e)
                {
                    results.Add(new PostSummary
                    {
                        Message = data.Item1,
                        Succeeded = false,
                        Exception = e,
                        Response = null
                    });
                }
            }
        }

        result.Data = results;
        result.Success = results.All(r => r.Succeeded);
        result.Exception = results.Any(r => r.Exception != null)
            ? new AggregateException("Some messages could not be posted", results.Where(r => r.Exception != null).Select(r => r.Exception!))
            : null;

        return results.All(r => r.Succeeded);
    }
}