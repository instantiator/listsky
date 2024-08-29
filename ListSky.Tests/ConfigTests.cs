using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
public class ConfigTests
{
    [TestMethod]
    public void Config_IsValid()
    {
        var config = Config.FromEnv();
        Assert.IsNotNull(config);
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.Server_AT));
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.AccountName_AT));
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.AppPassword_AT));
        Assert.IsFalse(string.IsNullOrWhiteSpace(config.Path_AllListsMetadataJson));
        Assert.IsTrue(File.Exists(config.Path_AllListsMetadataJson));
        Assert.IsNotNull(config.AllListData);
        Assert.IsNotNull(config.AllListData.Lists);
        Assert.IsTrue(config.AllListData.Lists.Count() > 0);
        Assert.AreEqual("Test", config.AllListData.Lists.ElementAt(0).Title);
    }

    [TestMethod]
    public void ListCSV_Readable()
    {
        var config = Config.FromEnv();
        var list = config.ReadList(config.AllListData.Lists.ElementAt(0).Path_CSV);
        Assert.IsNotNull(list);
        Assert.AreEqual(1, list.Count());

        var entry = list.ElementAt(0);
        Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Name));
        Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Description));
        Assert.AreEqual(ListEntryType.Individual, entry.Type);
        Assert.IsFalse(string.IsNullOrWhiteSpace(entry.AccountName_BlueSky));
    }
}