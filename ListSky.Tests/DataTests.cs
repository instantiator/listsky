using System.Text.Json;
using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
public class DataTests
{
    [TestMethod]
    public async Task AllEntries_HaveRequiredFields()
    {
        var config = Config.FromEnv();
        foreach (var entry in config.AllListData.Lists.SelectMany(l => config.ReadList(l.Path_CSV)))
        {
            Assert.IsNotNull(entry.Type, "Entry Type is null for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Name), "Entry Name is empty for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Description), "Entry Description is empty for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.AccountName_BlueSky), "Entry AccountName_BlueSky is empty for: " + JsonSerializer.Serialize(entry));
        }
    }

    [TestMethod]
    public async Task AT_PublishedLists_Exist()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();

        var atLists = await connection.GetListsAsync();
        foreach (var list in config.AllListData.Lists)
        {
            Assert.AreEqual(1, atLists.Count(l => l.Uri.Pathname.EndsWith($"/{list.ListId}")),
                $"List not found: {list.ListId}");
        }
    }

    [TestMethod]
    public async Task AT_ListEntries_ExistOnSocialNetwork()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();

        foreach (var entry in config.AllListData.Lists.SelectMany(l => config.ReadList(l.Path_CSV)))
        {
            var person = await connection.FindPersonByHandleAsync(entry.AccountName_BlueSky);
            Assert.IsNotNull(person, $"Person not found: {entry.AccountName_BlueSky}");
        }
    }

}