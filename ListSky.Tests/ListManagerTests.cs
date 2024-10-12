using ListSky.Lib.BlueSky.ListManagement;
using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
[TestCategory("BlueSky")]
[TestCategory("Integration")]
public class ListManagerTests : AbstractATConnectedTests
{

    [TestMethod]
    public async Task ListManager_CanIdentifyAdditionsAndDeletions()
    {
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
            var opinion = BlueSkyListManager.Compare(authoritativeList, foundList);

            Assert.AreEqual(authoritativeList.Single(), opinion.ToAdd.Single());
            Assert.AreEqual(config.AccountName_AT, opinion.ToDelete.Single().Subject.Handle);
        }
        finally
        {
            await connection.DeleteListAsync(list.Uri);
        }
    }

}