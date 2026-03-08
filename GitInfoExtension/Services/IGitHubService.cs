namespace GitInfoExtension.Services;

using GitInfoExtension.Models;

internal interface IGitHubService
{
    Task<ItemsResult> GetOpenItemsAsync(
        string owner, string repo, string? personalAccessToken, CancellationToken cancellationToken);

    Task<IReadOnlyList<RepositorySummaryModel>> GetUserRepositoriesAsync(
        string personalAccessToken, CancellationToken cancellationToken);
}
