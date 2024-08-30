using ListSky.Lib.Connectors;
using ListSky.Lib.DTO;
using ListSky.Lib.ListManagement;

namespace ListSky.Tests;

[TestClass]
public class ListManagerTests
{
    [TestCleanup]
    public async Task CleanUp()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();
        await DeleteAllUnitTestLists(connection);
    }


    [TestMethod]
    public async Task ListManager_CanIdentifyAdditionsAndDeletions()
    {
        var config = Config.FromEnv();
        var connection = new ATConnection(config.Server_AT, config.AccountName_AT, config.AppPassword_AT);
        var session = await connection.ConnectAsync();

        // create list
        var list = await connection.CreateListAsync("Unit test list");

        Thread.Sleep(5000);

        try
        {
            // add person to list
            var subject = await connection.FindPersonByHandleAsync(config.AccountName_AT);
            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.Did);

            var addedSubject = await connection.AddPersonToListAsync(list.Uri, subject.Did);
            Assert.IsNotNull(addedSubject);
            Assert.IsNotNull(addedSubject.Uri);
            Assert.IsNotNull(addedSubject.Uri.Did);

            Thread.Sleep(5000);

            var authoritativeList = new List<ListEntry>()
            {
                new ListEntry()
                {
                    Type = ListEntryType.Individual,
                    AccountName_BlueSky = Guid.NewGuid().ToString(),
                    Name = "Unit Test person",
                    Description = "Unit Test person description"
                }
            };

            var foundList = await connection.GetListItemsAsync(list.Uri);

            // check list manager for actions
            var opinion = ListManager.Compare(authoritativeList, foundList);

            Assert.AreEqual(authoritativeList.Single(), opinion.ToAdd.Single());
            Assert.AreEqual(config.AccountName_AT, opinion.ToDelete.Single().Subject.Handle);
        }
        finally
        {
            await connection.DeleteListAsync(list.Uri);
        }
    }

    private async Task DeleteAllUnitTestLists(ATConnection connection)
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