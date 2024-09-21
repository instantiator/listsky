using System.Text.Json;
using ListSky.Lib.BlueSky.ListManagement;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
[TestCategory("Config")]
public class DataTests : AbstractATConnectedTests
{
    [TestMethod]
    public void AllLists_HaveRequiredFields()
    {
        var config = Config.FromEnv();
        foreach (var list in config.AllListData.Lists)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.ListId), "ListId is empty for: " + JsonSerializer.Serialize(list));
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.Title), "List Title is null for: " + JsonSerializer.Serialize(list));
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.Slug), "List Slug is empty for: " + JsonSerializer.Serialize(list));
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.Path_CSV), "List Path_CSV is empty for: " + JsonSerializer.Serialize(list));
            // Assert.IsFalse(string.IsNullOrWhiteSpace(list.Description), "List Description is empty for: " + JsonSerializer.Serialize(list));
        }
    }

    [TestMethod]
    public void AllEntries_HaveRequiredFields()
    {
        var config = Config.FromEnv();
        foreach (var entry in config.AllListData.Lists.SelectMany(l => config.ReadList(l.Path_CSV)))
        {
            Assert.IsNotNull(entry.Type, "Entry Type is null for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Name), "Entry Name is empty for: " + JsonSerializer.Serialize(entry));
            Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Description), "Entry Description is empty for: " + JsonSerializer.Serialize(entry));
            // Assert.IsFalse(string.IsNullOrWhiteSpace(entry.AccountName_BlueSky), "Entry AccountName_BlueSky is empty for: " + JsonSerializer.Serialize(entry));
        }
    }

    [TestMethod]
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
    public async Task AT_NewListEntries_ExistOnSocialNetwork()
    {
        foreach (var list in config.AllListData.Lists)
        {
            var authoritativeEntries = config.ReadList(list.Path_CSV);
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

}