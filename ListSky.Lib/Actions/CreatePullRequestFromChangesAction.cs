using ListSky.Lib.GitHub;
using ListSky.Lib.IO;
using Octokit;

namespace ListSky.Lib.Actions;

public class CreatePullRequestsFromExternalSourceReportsAction : AbstractAction<IEnumerable<PullRequestReport>>
{
    private readonly IEnumerable<ExternalSourceReport> reports;
    private readonly Dictionary<string, ExternalSourceReport> combinedReports;
    private readonly GitHubIntegration github;

    public CreatePullRequestsFromExternalSourceReportsAction(Config.Config config, IEnumerable<ExternalSourceReport> reports) : base(config)
    {
        this.reports = reports;
        this.combinedReports = ExternalSourceReportUtils.CombineReports(reports);
        this.github = new GitHubIntegration(config);
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<PullRequestReport>> result)
    {
        foreach (var report in reports.Where(r => r.ContainsChanges))
        {
            // calculate changes to the list
            var filename = report.ListMetadata.Path_CSV;
            var currentListEntries = CsvListIO.ReadFile(filename);
            var newListEntries = ExternalSourceReportUtils.ApplyChanges(currentListEntries, report);
            var newListContent_CSV = CsvListIO.GenerateFileContent(newListEntries);

            // create a new branch for this list
            var title = GenTitle(report);
            var body = GenDescription(report);
            var branch = GenBranch(report);

            // create a change set / commit on the branch for this list, with the new CSV
            // see https://laedit.net/2016/11/12/GitHub-commit-with-Octokit-net.html
            var branchReference = github.GetBranchReferenceAsync(branch);
            var updateChangeSet = await github.CreateUpdateChangeSet(
                branch, 
                filename, 
                newListContent_CSV, 
                $"{title}\n\n{body}");

            // TODO: create a pull request
        }
        return true;
    }

    public static string GenTitle(ExternalSourceReport report) => $"Auto update {report.ListMetadata.Slug}: {report.ListMetadata.Title}";

    public static string GenDescription(ExternalSourceReport report) => $"This pull request updates the {report.ListMetadata.Title} list with changes from an external source.\n\n" +
            $"## Changes\n\n" +
            $"- **Added**\n" +
            $"{string.Join("\n", report.Add.Select(e => $"  - {e.Name}"))}\n\n" +
            $"- **Updated**\n" +
            $"{string.Join("\n", report.Update.Select(e => $"  - {e.Name}"))}\n\n" +
            $"- **Removed**\n" +
            $"{string.Join("\n", report.Remove.Select(e => $"  - {e.Name}"))}\n\n" +
            $"_Please review these changes and merge if appropriate._";

    public static string GenBranch(ExternalSourceReport report) => $"auto-update-{report.ListMetadata.Slug}";
}
