using ListSky.Lib.Config;
using ListSky.Lib.GitHub;

namespace ListSky.Tests;

[TestClass]
public class GitHubIntegrationTests
{
    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public void GitHubIntegration_Can_CreateClient()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);
        var client = github.GetClient();
        Assert.IsNotNull(client);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public async Task GitHubIntegration_Can_GetPRs()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);
        var prs = await github.GetPRsAsync();
        Assert.IsNotNull(prs);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public async Task GitHubIntegration_Can_GetMainHeadCommit()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);
        var commit = await github.GetMainHeadCommitAsync();
        Assert.IsNotNull(commit);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public async Task GitHubIntegration_Can_GetBranchReference()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);
        var branchReference = await github.GetBranchReferenceAsync("test-branch");
        Assert.IsNotNull(branchReference);
    }
}