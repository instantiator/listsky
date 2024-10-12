using Octokit;
using Org.BouncyCastle.Asn1.Cmp;

namespace ListSky.Lib.GitHub;

public class GitHubIntegration(Config.Config config)
{
    protected string Owner => config.GITHUB_USER;
    protected string Repo => config.GITHUB_REPO;
    protected string Token => config.GITHUB_TOKEN!;

    private GitHubClient? _client;
    public GitHubClient GetClient()
    {
        if (_client == null)
        {
            _client = new GitHubClient(new ProductHeaderValue("ListSky"));
            _client.Credentials = new Credentials(config.GITHUB_TOKEN);
        }
        return _client!;
    }

    public async Task<IEnumerable<PullRequest>> GetPRsAsync() =>
        await GetClient().PullRequest.GetAllForRepository(Owner, Repo);

    public async Task<Commit> GetMainHeadCommitAsync()
    {
        var reference = await GetClient().Git.Reference.Get(Owner, Repo, "heads/main");
        var commit = await GetClient().Git.Commit.Get(Owner, Repo, reference.Object.Sha);
        return commit;
    }

    public async Task<Reference> GetBranchReferenceAsync(string branch)
    {
        var client = GetClient();
        var branchReferences = await client.Git.Reference.GetAll(Owner, Repo);
        var branchReference = branchReferences.SingleOrDefault(r => r.Ref == $"refs/heads/{branch}");
        if (branchReference == null)
        {
            var mainHeadCommit = await GetMainHeadCommitAsync();
            var newBranchReference = new NewReference($"refs/heads/{branch}", mainHeadCommit.Sha);
            branchReference = await client.Git.Reference.Create(Owner, Repo, newBranchReference);
        }
        return branchReference;
    }

    public async Task<RepositoryContentChangeSet> CreateUpdateChangeSet(string branch, string filename, string content, string message)
    {
        var mainHeadCommit = await GetMainHeadCommitAsync();
        return await GetClient()
                .Repository
                .Content
                .UpdateFile(
                    owner: Owner,
                    name: Repo,
                    path: filename,
                    new UpdateFileRequest(
                        message,
                        content,
                        mainHeadCommit.Sha,
                        branch));
    }
}