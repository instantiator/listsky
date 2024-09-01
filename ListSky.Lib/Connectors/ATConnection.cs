using System.Diagnostics.CodeAnalysis;
using FishyFlip;
using FishyFlip.Models;
using FishyFlip.Tools;

namespace ListSky.Lib.Connectors;

public class ATConnection : IDisposable
{
    private const int RATE_ms = 1000;
    private static DateTime lastAction = DateTime.MinValue;

    private string server;
    private string account;
    private string password;

    private ATProtocol protocol;
    private Session? session;

    public ATConnection(string server, string account, string password)
    {
        this.server = server;
        this.account = account;
        this.password = password;

        this.protocol = new ATProtocolBuilder()
            .EnableAutoRenewSession(true)
            .WithInstanceUrl(new Uri("https://" + server))
            .Build();
    }

    public bool Connected => session != null;

    private static async Task RateLimit()
    {
        var now = DateTime.Now;
        var elapsed = now - lastAction;
        if (elapsed.TotalMilliseconds < RATE_ms)
        {
            var delay = RATE_ms - (int)elapsed.TotalMilliseconds;
            await Task.Delay(delay);
        }
    }

    public async Task<Session?> ConnectAsync()
    {
        await RateLimit();
        var result = await protocol.Server.CreateSessionAsync(account, password, CancellationToken.None);
        result.Switch(session =>
        {
            this.session = session;
        },
        error =>
        {
            this.session = null;
            throw new Exception($"Error: {error.StatusCode} {error.Detail}");
        });

        return this.session;
    }

    public void Disconnect()
    {
        session = null;
    }

    public void Dispose()
    {
        Disconnect();
    }

    [MemberNotNull(nameof(session))]
    private void RequireConnected()
    { 
        if (!Connected) throw new Exception("Not connected");
        if (session == null) throw new Exception("Connected, but session is null");
    }

    public async Task<IEnumerable<ListView>> GetListsAsync()
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Graph.GetListsAsync(session.Did);
        var lists = result.HandleResult()!.Lists;
        return lists;
    }

    public async Task<IEnumerable<ListItemView>> GetListItemsAsync(ATUri listUri)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Graph.GetListAsync(listUri);
        var list = result.HandleResult()!;
        return list.Items;
    }

    public async Task<RecordRef> CreateListAsync(string name, string? description = null)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Repo.CreateCurateListAsync(name, description ?? "List created by ListSky");
        var record = result.HandleResult()!;
        return record;
        // return record.Uri.Pathname.Split('/').Last();
    }

    public async Task<IEnumerable<ListItemView>> FindSubjectInList(ATUri listUri, ATDid subjectDid)
    {
        RequireConnected();
        var listItems = await GetListItemsAsync(listUri);
        return listItems.Where(item => item.Subject.Did?.Handler == subjectDid.Handler);
    }

    public async Task<Success> DeleteListAsync(ATUri listUri)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Repo.DeleteListAsync(listUri.Rkey);
        return result.HandleResult()!;
    }

    public async Task<RecordRef> AddPersonToListAsync(ATUri listUri, ATDid subject)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Repo.CreateListItemAsync(subject, listUri);
        var record = result.HandleResult()!;
        return record;
    }

    public async Task<Success> RemovePersonFromListAsync(ATUri listUri, ATDid subjectDid)
    {
        RequireConnected();
        var listItems = await GetListItemsAsync(listUri);
        var removalRkey = listItems.First(item => item.Subject.Did!.Handler == subjectDid.Handler).Uri.Rkey;
        await RateLimit();
        var result = await protocol.Repo.DeleteListItemAsync(removalRkey);
        return result.HandleResult()!;
    }

    public async Task<HandleResolution?> FindPersonByHandleAsync(string handle)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Identity.ResolveHandleAsync(ATHandle.Create(handle)!);
        return result.HandleResult();
    }

    public async Task<CreatePostResponse> PostAsync(string message)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Repo.CreatePostAsync(message);
        return result.HandleResult()!;
    }

    public async Task<Success> DeletePostAsync(ATUri uri)
    {
        RequireConnected();
        await RateLimit();
        var result = await protocol.Repo.DeletePostAsync(uri.Rkey);
        return result.HandleResult()!;
    }

    // private async Task<ActorRecord?> GetProfileViaHandle()
    // {
    //     var profile = (await protocol.Identity.ResolveHandleAsync(ATHandle.Create(account)!)).HandleResult();
    //     return await GetProfileViaATDID(profile?.Did!);
    // }

    // private async Task<ActorRecord?> GetProfileViaATDID(ATDid? did = null)
    // {
    //     return (await protocol.Repo.GetActorAsync(did)).HandleResult();
    // }
}