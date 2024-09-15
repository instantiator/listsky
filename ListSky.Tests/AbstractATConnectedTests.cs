using FishyFlip.Models;
using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

public abstract class AbstractATConnectedTests
{
    protected static Config config = null!;
    protected static ATConnection connection = null!;
    protected static Session? session;

    [TestInitialize]
    public async Task InitTest()
    {
        config = config ?? Config.FromEnv();
        connection = connection ?? new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        session = session ?? await connection.ConnectAsync();
    } 

    [TestCleanup]
    public async Task CleanUp()
    {
        await DeleteAllUnitTestLists();
    }

    protected async Task DeleteAllUnitTestLists()
    {
        var allLists = await connection.GetListsAsync();
        var deleteLists = allLists.Where(l => l.Name.StartsWith("Unit test"));
        var deleted = 0;
        foreach (var deleteList in deleteLists)
        {
            var deleteListOk = await connection.DeleteListAsync(deleteList.Uri);
            if (deleteListOk != null) deleted++;
        }
        Assert.AreEqual(deleteLists.Count(), deleted);
    }

}