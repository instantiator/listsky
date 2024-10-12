using System.Text.Json;
using ListSky.Lib.BlueSky.ListManagement;
using ListSky.Lib.Config;
using ListSky.Lib.IO;

namespace ListSky.Tests;

[TestClass]
public class DataTests : AbstractATConnectedTests
{
    [TestMethod]
    [TestCategory("Config")]
    public void AllLists_HaveRequiredFields()
    {
        var config = Config.FromEnv();
        foreach (var list in config.AllListData.Lists)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.ListId), "ListId is empty for: " + JsonSerializer.Serialize(list));
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.Title), "List Title is null for: " + JsonSerializer.Serialize(list));
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.Slug), "List Slug is empty for: " + JsonSerializer.Serialize(list));
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.Path_CSV), "List Path_CSV is empty for: " + JsonSerializer.Serialize(list));
        }
    }

    [TestMethod]
    [TestCategory("Config")]
    public void AllEntries_HaveRequiredFields()
    {
        var config = Config.FromEnv();
        foreach (var entry in config.AllListData.Lists.SelectMany(l => CsvListIO.ReadFile(l.Path_CSV)))
        {
            Assert.IsNotNull(entry.Type, "Entry Type is null for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Name), "Entry Name is empty for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Description), "Entry Description is empty for: " + JsonSerializer.Serialize(entry));
        }
    }

    [TestMethod]
    [TestCategory("Config")]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task AT_PublishedLists_Exist()
    {
        var atLists = await connection.GetListsAsync();
        foreach (var list in config.AllListData.Lists)
        {
            Assert.AreEqual(1, atLists.Count(l => l.Uri.Pathname.EndsWith($"/{list.ListId}")),
                $"List not found: {list.ListId}");
        }
    }

    [TestMethod]
    [TestCategory("Config")]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task AT_NewListEntries_ExistOnSocialNetwork()
    {
        foreach (var list in config.AllListData.Lists)
        {
            var authoritativeEntries = CsvListIO.ReadFile(list.Path_CSV);
            var allFoundLists = await connection.GetListsAsync();
            var foundList = allFoundLists.Single(l => l.Uri.Pathname.EndsWith($"/{list.ListId}"));
            var foundItems = await connection.GetListItemsAsync(foundList.Uri);
            var actions = ListManager.Compare(authoritativeEntries, foundItems);

            foreach (var newEntry in actions.ToAdd)
            {
                var person = await connection.FindPersonByHandleAsync(newEntry.AccountName_BlueSky);
                Assert.IsNotNull(person, $"Person not found: {newEntry.AccountName_BlueSky}");
            }
        }
    }

    [TestMethod]
    [TestCategory("Config")]
    [TestCategory("Integration")]
    public async Task ExternalSources_UrisExist()
    {
        foreach (var list in config.AllListData.Lists)
        {
            if (list.ExternalSources_CSV == null) continue;
            foreach (var source in list.ExternalSources_CSV)
            {
                var ok = Uri.TryCreate(source, UriKind.Absolute, out var uri);
                Assert.IsTrue(ok && uri != null, $"{list.Slug} has invalid external uri: {source}");
                
                var entries = await CsvListIO.ReadUriAsync(uri);
                Assert.IsNotNull(entries);
                Assert.IsTrue(entries.Count() > 0);
            }
        }
    }
}