using FishyFlip;
using FishyFlip.Models;
using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
public class ATConnectorTests
{
    [TestMethod]
    public async Task ATConnector_CanConnect()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();
        Assert.IsNotNull(session);
        Assert.IsTrue(connection.Connected);
    }

    [TestMethod]
    public async Task ATConnector_CanGetLists()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();
        var lists = await connection.GetListsAsync();
        Assert.IsNotNull(lists);
    }

    [TestMethod]
    public async Task ATConnector_CanCreateList()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();
        var list = await connection.CreateListAsync("Unit test list");
        Assert.IsNotNull(list);

        var ok = await connection.DeleteListAsync(list.Uri);
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task ATConnector_CanAddAndRemovePersonFromList()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();
        // var lists = await connection.GetLists();

        var list = await connection.CreateListAsync("Unit test list");

        // TODO: add person to list

        // TODO: find that person in list

        // TODO: remove person from list

        // TODO: establish that person is not in the list

        // TODO: delete the list

        Assert.IsTrue(false);
    }

}