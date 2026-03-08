namespace GitInfoExtension.Models;

internal record ItemsResult(
    IReadOnlyList<IssueModel> Issues,
    IReadOnlyList<PullRequestModel> PullRequests);
