using System.ComponentModel;

namespace ListSky.Tests;

[TestClass]
public class ATConnectorTests : AbstractATConnectedTests
{
    [TestMethod]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public void ATConnector_CanConnect()
    {
        Assert.IsNotNull(session);
        Assert.IsTrue(connection.Connected);
    }

    [TestMethod]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task ATConnector_CanGetLists()
    {
        var lists = await connection.GetListsAsync();
        Assert.IsNotNull(lists);
    }

    [TestMethod]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task ATConnector_CanCreateList()
    {
        var list = await connection.CreateListAsync("Unit test list");
        Assert.IsNotNull(list);

        var ok = await connection.DeleteListAsync(list.Uri);
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task ATConnector_CanAddAndRemovePersonFromList()
    {
        // create list
        var list = await connection.CreateListAsync("Unit test list");

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

            // find that person in list
            Thread.Sleep(5000);

            var listWithPerson = await connection.GetListItemsAsync(list.Uri);
            Assert.AreEqual(1, listWithPerson.Count());

            var personInList = listWithPerson.SingleOrDefault(p => p.Subject.Did!.Handler.Equals(addedSubject.Uri.Did.Handler));
            Assert.IsNotNull(personInList);

            var subjectInList = await connection.FindSubjectInList(list.Uri, subject.Did);
            Assert.AreEqual(1, subjectInList.Count());

            // remove person from list
            var removeOk = await connection.RemovePersonFromListAsync(list.Uri, subject.Did);
            Assert.IsNotNull(removeOk);

            // establish that person is not in the list
            var listWithoutPerson = await connection.GetListItemsAsync(list.Uri);
            var personNotInList = listWithoutPerson.FirstOrDefault(p => p.Subject.Did == subject.Did);
            Assert.IsNull(personNotInList);

        }
        finally
        {
            // delete the list
            var deleteOk = await connection.DeleteListAsync(list.Uri);
            Assert.IsNotNull(deleteOk);
        }
    }

    [TestMethod]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task ATConnector_CanPost()
    {
        var result = await connection.PostAsync("Unit test message");
        Assert.IsNotNull(result);
        Thread.Sleep(5000);

        var deleted = await connection.DeletePostAsync(result.Uri!);
        Assert.IsNotNull(deleted);
    }

    [TestMethod]
    [TestCategory("BlueSky")]
    [TestCategory("Integration")]
    public async Task ATConnector_CanPostWithFacets()
    {
        var result = await connection.PostAsync("Unit test message @instantiator.bsky.social has a blog at https://instantiator.dev");
        Assert.IsNotNull(result);
        Thread.Sleep(5000);

        var deleted = await connection.DeletePostAsync(result.Uri!);
        Assert.IsNotNull(deleted);
    }

}