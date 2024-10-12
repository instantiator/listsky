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
        try
        {
            var branchReference = await github.GetBranchReferenceAsync("unit-test-branch");
            Assert.IsNotNull(branchReference);
        }
        finally
        {
            await github.DeleteBranchAsync("unit-test-branch");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public async Task GitHubIntegration_Can_DeleteBranch()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);

        var branchReference = await github.GetBranchReferenceAsync("unit-test-branch-for-deletion", true);
        Assert.IsNotNull(branchReference);

        await github.DeleteBranchAsync("unit-test-branch-for-deletion");
        branchReference = await github.GetBranchReferenceAsync("unit-test-branch-for-deletion", false);
        Assert.IsNull(branchReference);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public async Task GitHubIntegration_Can_ModifyFileInBranch()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);
        try
        {
            var branchReference = await github.GetBranchReferenceAsync("unit-test-branch-file-modification");
            Assert.IsNotNull(branchReference);

            var changes = await github.ModifyFile(
                "unit-test-branch-file-modification", 
                "README.md", 
                "This is a very short README now.", 
                "Shortened the README");

            Assert.IsNotNull(changes);
            Assert.AreEqual("README.md", changes.Content.Path);
        }
        finally
        {
            await github.DeleteBranchAsync("unit-test-branch-file-modification");
        }
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("GitHub")]
    public async Task GitHubIntegration_CanCreatePR_FromBranch()
    {
        var config = Config.FromEnv();
        var github = new GitHubIntegration(config);
        try
        {
            var branchReference = await github.GetBranchReferenceAsync("unit-test-branch-for-pr");
            Assert.IsNotNull(branchReference);

            var changes = await github.ModifyFile(
                "unit-test-branch-for-pr", 
                "README.md", 
                "This is a very short README now.", 
                "Shortened the README");

            Assert.IsNotNull(changes);

            var pr = await github.CreateOrUpdatePRAsync(
                "unit-test-branch-for-pr", 
                "main", 
                "[unit test] Shorten the README", 
                "The README is now a lot shorter.");
            Assert.IsNotNull(pr);
            Assert.AreEqual("[unit test] Shorten the README", pr.Title);
            Assert.AreEqual(1, pr.Commits);
            Assert.AreEqual(1, pr.ChangedFiles);
        
            var found = (await github.GetPRsAsync()).FirstOrDefault(p => p.Title == "[unit test] Shorten the README");
            Assert.IsNotNull(found);

            await github.ClosePRAsync(pr);
            var notFound = (await github.GetPRsAsync()).FirstOrDefault(p => p.Title == "[unit test] Shorten the README");
            Assert.IsNull(notFound);
        }
        finally
        {
            await github.DeleteBranchAsync("unit-test-branch-for-pr");
        }
    }

}