using ListSky.Lib.GitHub;
using ListSky.Lib.IO;
using Octokit;

namespace ListSky.Lib.Actions;

public class CreatePullRequestsAction : AbstractAction<IEnumerable<PullRequestReport>>
{
    private readonly IEnumerable<ExternalSourceReport> reports;
    private readonly Dictionary<string, ExternalSourceReport> combinedReports;
    private readonly GitHubIntegration github;

    public CreatePullRequestsAction(Config.Config config, IEnumerable<ExternalSourceReport> reports) : base(config)
    {
        this.reports = reports;
        this.combinedReports = ExternalSourceReportUtils.CombineReports(reports);
        this.github = new GitHubIntegration(config);
    }

    protected override async Task<bool> ExecuteImplementationAsync(ActionResult<IEnumerable<PullRequestReport>> result)
    {
        foreach (var report in reports.Where(r => r.ContainsChanges))
        {
            result.Outputs.Add($"Checking list: {report.ListMetadata.Slug}");
            // calculate changes to the list
            var filename = report.ListMetadata.Path_CSV;
            var currentListEntries = CsvListIO.ReadFile(filename);
            result.Outputs.Add($" - Preparing to alter: {filename}");
            result.Outputs.Add($" - Adding {report.Add.Count()}, updating {report.Update.Count()}, removing {report.Remove.Count()} entries");
            var newListEntries = ExternalSourceReportUtils.ApplyChanges(currentListEntries, report);
            var newListContent_CSV = CsvListIO.GenerateFileContent(newListEntries);

            // create a new branch for this list
            var title = GenTitle(report);
            var body = GenDescription(report);
            var branch = GenBranch(report);

            // create a change set / commit on the branch for this list, with the new CSV
            var branchReference = await github.GetBranchReferenceAsync(branch);
            if (branchReference == null) throw new Exception("Failed to create branch reference");
            result.Outputs.Add($" - Branch {branch}: {branchReference.Url}");
            var updateChangeSet = await github.ModifyFile(
                branch, 
                filename, 
                newListContent_CSV, 
                $"{title}\n\n{body}");

            // if updateChangeSet == null, no need to bother updating the PR
            if (updateChangeSet == null)
            {
                result.Outputs.Add($" - No changes to: {filename}");
                continue;
            }

            result.Outputs.Add($" - Modified file: {filename}");    
            result.Outputs.Add($" - Created change set: {updateChangeSet.Commit.Url}");

            // create a pull request from the altered branch
            var allPRs = await github.GetPRsAsync();
            var existingPR = allPRs.FirstOrDefault(p => p.Head.Ref == branch);
            var newPR = await github.CreateOrUpdatePRAsync(branch, "main", title, body, existingPR);
            result.Outputs.Add($" - {(existingPR == null ? "Created" : "Updated")} PR #{newPR.Number}: {newPR.HtmlUrl}");
        }
        return true;
    }

    public static string GenTitle(ExternalSourceReport report) => $"Auto update: {report.ListMetadata.Title}";

    public static string GenDescription(ExternalSourceReport report) => $"This pull request updates the {report.ListMetadata.Title} list with changes from external sources.\n\n" +
            $"* List slug: {report.ListMetadata.Slug}\n" +
            $"* CSV path: {report.ListMetadata.Path_CSV}\n" +
            $"* List id: {report.ListMetadata.ListId}\n" +
            $"* Sources:\n" +
            report.ListMetadata.ExternalSources_CSV?.Select(source => $"  * {source}\n") +
            report.ListMetadata.ExternalSources_JSON?.Select(source => $"  * {source}\n") + "\n\n" +
            $"## Changes\n\n" +
            $"- **Added ({report.Add.Count()})**\n" +
            $"{string.Join("\n", report.Add.Select(e => $"  - {e.Name}"))}\n\n" +
            $"- **Updated ({report.Update.Count()})**\n" +
            $"{string.Join("\n", report.Update.Select(e => $"  - {e.Name}"))}\n\n" +
            $"- **Removed ({report.Remove.Count()})**\n" +
            $"{string.Join("\n", report.Remove.Select(e => $"  - {e.Name}"))}\n\n" +
            $"_Please review these changes and merge if appropriate._";

    public static string GenBranch(ExternalSourceReport report) => $"auto-update-{report.ListMetadata.Slug}";
}
