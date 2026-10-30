namespace GitInfoExtension.Services;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GitInfoExtension.Models;

internal class AzureDevOpsService : IAzureDevOpsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private HttpClient? _cachedClient;
    private string? _cachedKey;

    public async Task<ItemsResult> GetOpenItemsAsync(
        string organization, string project, string repositoryName,
        string? mainProject, string? tagFilter,
        string? statesFilter, string? assignedToFilter,
        string? pat, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(pat))
            return new ItemsResult([], []);

        var client = GetOrCreateClient(organization, pat);

        var pullRequests = await FetchPullRequestsAsync(
            client, organization, project, repositoryName, ct);

        var workItemProject = string.IsNullOrWhiteSpace(mainProject) ? project : mainProject;
        var effectiveTag = string.IsNullOrWhiteSpace(tagFilter) ? project : tagFilter;
        var issues = await FetchWorkItemsAsync(
            client, organization, workItemProject, effectiveTag,
            statesFilter, assignedToFilter, ct);

        return new ItemsResult(issues, pullRequests);
    }

    private async Task<IReadOnlyList<PullRequestModel>> FetchPullRequestsAsync(
        HttpClient client, string organization, string project,
        string repositoryName, CancellationToken ct)
    {
        var url = $"https://dev.azure.com/{PathSegment(organization)}/{PathSegment(project)}"
            + $"/_apis/git/repositories/{PathSegment(repositoryName)}"
            + "/pullrequests?searchCriteria.status=active&api-version=7.1";

        var response = await client.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<AzDoListResponse<AzDoPullRequest>>(json, JsonOptions);

        var pullRequests = new List<PullRequestModel>();
        if (result?.Value != null)
        {
            foreach (var pr in result.Value)
            {
                var prUrl = $"https://dev.azure.com/{PathSegment(organization)}/{PathSegment(project)}"
                    + $"/_git/{PathSegment(repositoryName)}/pullrequest/{pr.PullRequestId}";

                pullRequests.Add(new PullRequestModel
                {
                    DisplayNumber = $"PR !{pr.PullRequestId}",
                    Title = pr.Title ?? string.Empty,
                    DisplayInfo = $"by {pr.CreatedBy?.DisplayName ?? "unknown"}"
                        + $" on {pr.CreationDate:yyyy-MM-dd}",
                    Labels = string.Join(", ",
                        pr.Labels?.Select(l => l.Name ?? string.Empty)
                            .Where(n => !string.IsNullOrEmpty(n)) ?? []),
                    Url = prUrl,
                    DraftBadge = pr.IsDraft ? "DRAFT" : string.Empty,
                    BranchInfo = $"{pr.TargetRefName?.Replace("refs/heads/", "")} \u2190 "
                        + $"{pr.SourceRefName?.Replace("refs/heads/", "")}",
                });
            }
        }

        return pullRequests;
    }

    private async Task<IReadOnlyList<IssueModel>> FetchWorkItemsAsync(
        HttpClient client, string organization, string workItemProject,
        string tagFilter, string? statesFilter, string? assignedToFilter,
        CancellationToken ct)
    {
        var states = string.IsNullOrWhiteSpace(statesFilter)
            ? "'New','Active','Resolved'"
            : string.Join(",", statesFilter.Split(',')
                .Select(s => $"'{Wiql(s.Trim())}'"));

        var wiql = $"SELECT [System.Id] FROM WorkItems"
            + $" WHERE [System.TeamProject] = '{Wiql(workItemProject)}'"
            + $" AND [System.State] IN ({states})"
            + $" AND [System.Tags] CONTAINS '{Wiql(tagFilter)}'";

        if (!string.IsNullOrWhiteSpace(assignedToFilter))
        {
            wiql += assignedToFilter.Trim().Equals("@me", StringComparison.OrdinalIgnoreCase)
                ? " AND [System.AssignedTo] = @Me"
                : $" AND [System.AssignedTo] = '{Wiql(assignedToFilter.Trim())}'";
        }

        wiql += " ORDER BY [System.ChangedDate] DESC";

        var wiqlUrl = $"https://dev.azure.com/{PathSegment(organization)}/{PathSegment(workItemProject)}"
            + "/_apis/wit/wiql?api-version=7.1";

        var wiqlBody = JsonSerializer.Serialize(new { query = wiql });
        var wiqlContent = new StringContent(wiqlBody, Encoding.UTF8, "application/json");
        var wiqlResponse = await client.PostAsync(wiqlUrl, wiqlContent, ct);
        wiqlResponse.EnsureSuccessStatusCode();

        var wiqlJson = await wiqlResponse.Content.ReadAsStringAsync(ct);
        var wiqlResult = JsonSerializer.Deserialize<WiqlResponse>(wiqlJson, JsonOptions);

        var workItemIds = wiqlResult?.WorkItems?.Select(wi => wi.Id).ToList() ?? [];
        if (workItemIds.Count == 0)
            return [];

        var issues = new List<IssueModel>();
        const int batchSize = 200;

        for (var i = 0; i < workItemIds.Count; i += batchSize)
        {
            var batch = workItemIds.Skip(i).Take(batchSize);
            var ids = string.Join(",", batch);
            var detailUrl = $"https://dev.azure.com/{PathSegment(organization)}/{PathSegment(workItemProject)}"
                + $"/_apis/wit/workitems?ids={ids}&$expand=relations&api-version=7.1";

            var detailResponse = await client.GetAsync(detailUrl, ct);
            detailResponse.EnsureSuccessStatusCode();

            var detailJson = await detailResponse.Content.ReadAsStringAsync(ct);
            var detailResult = JsonSerializer.Deserialize<AzDoListResponse<AzDoWorkItem>>(
                detailJson, JsonOptions);

            if (detailResult?.Value != null)
            {
                foreach (var wi in detailResult.Value)
                {
                    var fields = wi.Fields;
                    var itemUrl = $"https://dev.azure.com/{PathSegment(organization)}/{PathSegment(workItemProject)}"
                        + $"/_workitems/edit/{wi.Id}";

                    issues.Add(new IssueModel
                    {
                        DisplayNumber = $"#{wi.Id}",
                        Title = fields?.Title ?? string.Empty,
                        DisplayInfo = $"assigned to {fields?.AssignedTo?.DisplayName ?? "unassigned"}"
                            + $" on {fields?.CreatedDate:yyyy-MM-dd}",
                        Labels = fields?.Tags ?? string.Empty,
                        Url = itemUrl,
                    });
                }
            }
        }

        return issues;
    }

    // Organization, project and repository are logical names ("My Project"): encode them as URL path segments.
    private static string PathSegment(string segment) => Uri.EscapeDataString(segment);

    // WIQL string literals are single-quoted; a quote inside a value is written twice.
    private static string Wiql(string value) => value.Replace("'", "''");

    private HttpClient GetOrCreateClient(string organization, string pat)
    {
        var key = $"{organization}:{pat}";
        if (_cachedClient != null && _cachedKey == key)
            return _cachedClient;

        _cachedClient?.Dispose();

        var client = new HttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _cachedClient = client;
        _cachedKey = key;
        return client;
    }

    // JSON response models for Azure DevOps REST API

    private class AzDoListResponse<T>
    {
        public int Count { get; set; }
        public List<T>? Value { get; set; }
    }

    private class AzDoPullRequest
    {
        public int PullRequestId { get; set; }
        public string? Title { get; set; }
        public AzDoIdentity? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsDraft { get; set; }
        public string? SourceRefName { get; set; }
        public string? TargetRefName { get; set; }
        public List<AzDoLabel>? Labels { get; set; }
    }

    private class AzDoIdentity
    {
        public string? DisplayName { get; set; }
    }

    private class AzDoLabel
    {
        public string? Name { get; set; }
    }

    private class WiqlResponse
    {
        public List<WiqlWorkItemRef>? WorkItems { get; set; }
    }

    private class WiqlWorkItemRef
    {
        public int Id { get; set; }
    }

    private class AzDoWorkItem
    {
        public int Id { get; set; }
        public AzDoWorkItemFields? Fields { get; set; }
    }

    private class AzDoWorkItemFields
    {
        [System.Text.Json.Serialization.JsonPropertyName("System.Title")]
        public string? Title { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("System.AssignedTo")]
        public AzDoIdentity? AssignedTo { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("System.CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("System.Tags")]
        public string? Tags { get; set; }
    }
}
