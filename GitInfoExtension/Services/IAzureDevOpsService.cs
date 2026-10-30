namespace GitInfoExtension.Services;

using GitInfoExtension.Models;

internal interface IAzureDevOpsService
{
    Task<ItemsResult> GetOpenItemsAsync(
        string organization, string project, string repositoryName,
        string? mainProject, string? tagFilter,
        string? statesFilter, string? assignedToFilter,
        string? pat, CancellationToken ct);
}
