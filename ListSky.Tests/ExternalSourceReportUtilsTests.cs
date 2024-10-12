using ListSky.Lib.DTO;
using ListSky.Lib.IO;

namespace ListSky.Tests;

[TestClass]
[TestCategory("Unit")]
public class ExternalSourceReportUtilsTests
{
    [TestMethod, TestCategory("Unit")]
    public void ExternalSourceReportUtils_CanDiffLists()
    {
        var metadata = new ListMetadata()
        {
            Description = "Test List",
            Title = "Test List",
            Slug = "test-list",
            ExternalSources_CSV = new List<string> { "https://import-list.test/test-list-1.csv" },
            Path_CSV = "test-list.csv",
            ListId = Guid.NewGuid().ToString(),
            Publish = true
        };

        var PROVIDENCE_0 = "https://import-list.test/test-list-0.csv";
        var PROVIDENCE_1 = "https://import-list.test/test-list-1.csv";


        var allEntries = new List<ListEntry>
        {
            new ListEntry { Name = "Test Entry 0", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-0.bsky.social", Providence = PROVIDENCE_0 },
            new ListEntry { Name = "Test Entry 1", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-1.bsky.social", Providence = PROVIDENCE_1 },
            new ListEntry { Name = "Test Entry 2", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-2.bsky.social", Providence = PROVIDENCE_1 },
            new ListEntry { Name = "Test Entry 3", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-3.bsky.social", Providence = PROVIDENCE_1 },
        };

        var incomingEntries = new List<ListEntry>
        {
            new ListEntry { Name = "Test Entry ONE", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-1.bsky.social", Providence = PROVIDENCE_1 },
            new ListEntry { Name = "Test Entry FOUR", Type = ListEntryType.Individual, AccountName_BlueSky = "test-entry-4.bsky.social", Providence = PROVIDENCE_1 },
        };

        var report = ExternalSourceReportUtils.DiffLists(metadata, PROVIDENCE_1, allEntries, incomingEntries);
        Assert.IsNotNull(report);

        Assert.AreEqual(1, report.Add.Count());
        Assert.AreEqual("Test Entry FOUR", report.Add.First().Name);

        Assert.AreEqual(1, report.Update.Count());
        Assert.AreEqual("Test Entry ONE", report.Update.First().Name);

        Assert.AreEqual(2, report.Remove.Count());
        Assert.IsTrue(report.Remove.Select(acc => acc.Name).Contains("Test Entry 2"));
        Assert.IsTrue(report.Remove.Select(acc => acc.Name).Contains("Test Entry 3"));
    }


}