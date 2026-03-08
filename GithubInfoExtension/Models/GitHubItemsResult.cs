namespace GithubInfoExtension.Models;

internal record GitHubItemsResult(
    IReadOnlyList<GitHubIssueModel> Issues,
    IReadOnlyList<GitHubPullRequestModel> PullRequests);
