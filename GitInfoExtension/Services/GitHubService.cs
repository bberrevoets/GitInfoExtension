namespace GitInfoExtension.Services;

using GitInfoExtension.Models;
using Octokit;

internal class GitHubService : IGitHubService
{
    public async Task<ItemsResult> GetOpenItemsAsync(
        string owner, string repo, string? personalAccessToken, CancellationToken cancellationToken)
    {
        var client = CreateClient(personalAccessToken);

        var issueRequest = new RepositoryIssueRequest
        {
            State = ItemStateFilter.Open,
        };

        var allItems = await client.Issue.GetAllForRepository(owner, repo, issueRequest);

        var issues = allItems
            .Where(i => i.PullRequest == null)
            .Select(i => new IssueModel
            {
                DisplayNumber = $"#{i.Number}",
                Title = i.Title,
                DisplayInfo = $"by {i.User?.Login ?? "unknown"} on {i.CreatedAt:yyyy-MM-dd}",
                Labels = string.Join(", ", i.Labels.Select(l => l.Name)),
                Url = i.HtmlUrl?.ToString() ?? string.Empty,
            })
            .ToList();

        // Build a lookup of labels from the Issues API for PRs
        var prItemsByNumber = allItems
            .Where(i => i.PullRequest != null)
            .ToDictionary(i => i.Number);

        var pullRequests = new List<PullRequestModel>();

        if (prItemsByNumber.Count > 0)
        {
            var prs = await client.PullRequest.GetAllForRepository(
                owner, repo, new PullRequestRequest { State = ItemStateFilter.Open });

            foreach (var pr in prs)
            {
                var labels = prItemsByNumber.TryGetValue(pr.Number, out var issueItem)
                    ? string.Join(", ", issueItem.Labels.Select(l => l.Name))
                    : string.Empty;

                pullRequests.Add(new PullRequestModel
                {
                    DisplayNumber = $"PR #{pr.Number}",
                    Title = pr.Title,
                    DisplayInfo = $"by {pr.User?.Login ?? "unknown"} on {pr.CreatedAt:yyyy-MM-dd}",
                    Labels = labels,
                    Url = pr.HtmlUrl?.ToString() ?? string.Empty,
                    DraftBadge = pr.Draft ? "DRAFT" : string.Empty,
                    BranchInfo = $"{pr.Base?.Ref ?? "?"} \u2190 {pr.Head?.Ref ?? "?"}",
                });
            }
        }

        return new ItemsResult(issues, pullRequests);
    }

    public async Task<IReadOnlyList<RepositorySummaryModel>> GetUserRepositoriesAsync(
        string personalAccessToken, CancellationToken cancellationToken)
    {
        var client = CreateClient(personalAccessToken);

        var currentUser = await client.User.Current();
        var repos = await client.Repository.GetAllForCurrent();

        var filteredRepos = repos
            .Where(r => r.Owner.Login.Equals(currentUser.Login, StringComparison.OrdinalIgnoreCase)
                        && r.OpenIssuesCount > 0)
            .OrderByDescending(r => r.UpdatedAt)
            .ToList();

        var results = new List<RepositorySummaryModel>();

        foreach (var repo in filteredRepos)
        {
            var allItems = await client.Issue.GetAllForRepository(
                repo.Owner.Login, repo.Name,
                new RepositoryIssueRequest { State = ItemStateFilter.Open });

            var issueCount = allItems.Count(i => i.PullRequest == null);
            var prCount = allItems.Count(i => i.PullRequest != null);

            var parts = new List<string>();
            if (issueCount > 0)
                parts.Add($"{issueCount} open issue{(issueCount == 1 ? "" : "s")}");
            if (prCount > 0)
                parts.Add($"{prCount} open PR{(prCount == 1 ? "" : "s")}");

            results.Add(new RepositorySummaryModel
            {
                FullName = repo.FullName,
                OpenItemsCount = string.Join(", ", parts),
                Description = repo.Description ?? string.Empty,
                Url = repo.HtmlUrl ?? string.Empty,
            });
        }

        return results;
    }

    private static GitHubClient CreateClient(string? personalAccessToken)
    {
        var client = new GitHubClient(new ProductHeaderValue("GitInfoExtension"));

        if (!string.IsNullOrWhiteSpace(personalAccessToken))
        {
            client.Credentials = new Credentials(personalAccessToken);
        }

        return client;
    }
}
