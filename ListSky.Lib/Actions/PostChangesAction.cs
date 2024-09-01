
using FishyFlip.Models;
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;

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

    public PostChangesAction(Config config, ListResolutions resolutions) : base(config)
    {
        this.resolutions = resolutions;
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<PostSummary>> result)
    {
        var results = new List<PostSummary>();

        foreach (var listResolution in resolutions)
        {
            foreach (var addition in listResolution.Value.ToAdd)
            {
                var message = $"➕ Added {addition.Name} (@{addition.AccountName_BlueSky}) to list: {listResolution.Key.Title}";
                try
                {
                    var response = await connection.PostAsync(message);
                    results.Add(new PostSummary
                    {
                        Message = message,
                        Response = response,
                        Succeeded = response != null
                    });
                }
                catch (Exception e)
                {
                    results.Add(new PostSummary
                    {
                        Message = message,
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