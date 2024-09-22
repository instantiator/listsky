using ListSky.Lib.Config;

namespace ListSky.Tests;

[TestClass]
[TestCategory("Config")]
public class ConfigTests
{
    [TestMethod]
    public void Config_IsValid()
    {
        var config = Config.FromEnv();
        Assert.IsNotNull(config);
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.Server_AT), "Server_AT is empty");
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.AccountName_AT), "AccountName_AT is empty");
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.AppPassword_AT), "AppPassword_AT is empty");
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.Path_AllListsMetadataJson), "Path_AllListsMetadataJson is empty");
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.GITHUB_REPO), "GITHUB_REPO is empty");
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.GITHUB_USER), "GITHUB_USER is empty");
        Assert.IsTrue(File.Exists(config.Path_AllListsMetadataJson), $"File not found at: {config.Path_AllListsMetadataJson}");
        Assert.IsNotNull(config.AllListData, "AllListData is null");
        Assert.IsNotNull(config.AllListData.Lists, "AllListData.Lists is null");
        Assert.IsTrue(config.AllListData.Lists.Count() > 0, "AllListData.Lists is empty");
    }

}