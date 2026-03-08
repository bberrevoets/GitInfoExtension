namespace GithubInfoExtension.Services;

using GithubInfoExtension.Models;

internal interface IGitHubService
{
    Task<GitHubItemsResult> GetOpenItemsAsync(
        string owner, string repo, string? personalAccessToken, CancellationToken cancellationToken);

    Task<IReadOnlyList<GitHubRepositorySummaryModel>> GetUserRepositoriesAsync(
        string personalAccessToken, CancellationToken cancellationToken);
}
