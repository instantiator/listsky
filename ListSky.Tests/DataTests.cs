using System.Text.Json;
using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
public class DataTests
{
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
                JsonSerializer.Serialize(atLists));
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