using ListSky.Lib.DTO;

namespace ListSky.Tests;

[TestClass]
[TestCategory("Unit")]
public class DTOTests
{
    [TestMethod]
    public void ListEntry_IsProbably_MatchesOnAccountNames()
    {
        var entry1a = new ListEntry { Name = "Test Entry ONE", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-1.bsky.social", Providence = "https://import-list.test/test-list-1.csv" };
        var entry1b = new ListEntry { Name = "Test Entry BLOOP", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-1.bsky.social", Providence = "https://import-list.test/test-list-1.csv" };
        Assert.IsTrue(entry1b.IsProbably(entry1a));
        Assert.IsTrue(entry1a.IsProbably(entry1b));

        var entry4 = new ListEntry { Name = "Test Entry FOUR", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-4.bsky.social", Providence = "https://import-list.test/test-list-1.csv" };
        Assert.IsFalse(entry4.IsProbably(entry1a));
        Assert.IsFalse(entry1a.IsProbably(entry4));
    }

}