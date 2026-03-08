namespace GitInfoExtension;

using System.IO;
using System.Runtime.Serialization;
using GitInfoExtension.Models;
using GitInfoExtension.Services;
using GitInfoExtension.Settings;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;
using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.ProjectSystem.Query;

[DataContract]
internal class GitInfoToolWindowViewModel : NotifyPropertyChangedObject
{
    private readonly IGitRepositoryDetector _repoDetector;
    private readonly IGitHubService _gitHubService;
    private readonly VisualStudioExtensibility _extensibility;

    private CancellationTokenSource? _autoRefreshCts;
    private string? _lastKnownSolutionPath;
    private readonly SemaphoreSlim _loadDataSemaphore = new(1, 1);
    private const int SolutionMonitorIntervalSeconds = 3;

    private string _statusMessage = "Loading...";
    private string _issuesStatusMessage = string.Empty;
    private string _pullRequestStatusMessage = string.Empty;
    private string _repositoriesStatusMessage = string.Empty;
    private string _repositoryName = string.Empty;
    private string _repoListVisibility = "Collapsed";
    private string _repoDetailVisibility = "Visible";

    public GitInfoToolWindowViewModel(
        VisualStudioExtensibility extensibility,
        IGitRepositoryDetector repoDetector,
        IGitHubService gitHubService)
    {
        _extensibility = extensibility;
        _repoDetector = repoDetector;
        _gitHubService = gitHubService;
        RefreshCommand = new AsyncCommand(ExecuteRefreshAsync);
    }

    [DataMember]
    public ObservableList<IssueModel> Issues { get; } = new();

    [DataMember]
    public ObservableList<PullRequestModel> PullRequests { get; } = new();

    [DataMember]
    public ObservableList<RepositorySummaryModel> Repositories { get; } = new();

    [DataMember]
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    [DataMember]
    public string IssuesStatusMessage
    {
        get => _issuesStatusMessage;
        set => SetProperty(ref _issuesStatusMessage, value);
    }

    [DataMember]
    public string PullRequestStatusMessage
    {
        get => _pullRequestStatusMessage;
        set => SetProperty(ref _pullRequestStatusMessage, value);
    }

    [DataMember]
    public string RepositoriesStatusMessage
    {
        get => _repositoriesStatusMessage;
        set => SetProperty(ref _repositoriesStatusMessage, value);
    }

    [DataMember]
    public string RepositoryName
    {
        get => _repositoryName;
        set => SetProperty(ref _repositoryName, value);
    }

    [DataMember]
    public string RepoListVisibility
    {
        get => _repoListVisibility;
        set => SetProperty(ref _repoListVisibility, value);
    }

    [DataMember]
    public string RepoDetailVisibility
    {
        get => _repoDetailVisibility;
        set => SetProperty(ref _repoDetailVisibility, value);
    }

    [DataMember]
    public IAsyncCommand RefreshCommand { get; }

    private Task ExecuteRefreshAsync(object? parameter, CancellationToken cancellationToken)
    {
        return LoadDataAsync(cancellationToken);
    }

    public async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (!await _loadDataSemaphore.WaitAsync(0, cancellationToken))
            return; // another load already in progress

        try
        {
            Issues.Clear();
            PullRequests.Clear();
            Repositories.Clear();
            StatusMessage = string.Empty;
            IssuesStatusMessage = string.Empty;
            PullRequestStatusMessage = string.Empty;
            RepositoriesStatusMessage = string.Empty;

            var pat = await ReadPatAsync(cancellationToken);
            var solutionDir = await GetSolutionDirectoryAsync(cancellationToken);
            _lastKnownSolutionPath = solutionDir;

            if (solutionDir != null)
            {
                await LoadRepoDetailAsync(solutionDir, pat, cancellationToken);
            }
            else if (pat != null)
            {
                await LoadUserRepositoriesAsync(pat, cancellationToken);
            }
            else
            {
                RepoListVisibility = "Collapsed";
                RepoDetailVisibility = "Collapsed";
                StatusMessage = "No solution is open. Configure a PAT in Tools > Options "
                    + "> Git Info Extension to see your repositories. A VS restart "
                    + "may be needed after first install for settings to appear.";
                RepositoryName = string.Empty;
            }
        }
        catch (Octokit.RateLimitExceededException)
        {
            StatusMessage = "Rate limited by GitHub. Configure a PAT in Tools > Options.";
            RepoListVisibility = "Collapsed";
            RepoDetailVisibility = "Collapsed";
        }
        catch (Octokit.NotFoundException)
        {
            StatusMessage = "Repository not found. Check access or configure a PAT.";
            RepoListVisibility = "Collapsed";
            RepoDetailVisibility = "Collapsed";
        }
        catch (Octokit.AuthorizationException)
        {
            StatusMessage = "Invalid Personal Access Token.";
            RepoListVisibility = "Collapsed";
            RepoDetailVisibility = "Collapsed";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            RepoListVisibility = "Collapsed";
            RepoDetailVisibility = "Collapsed";
        }
        finally
        {
            _loadDataSemaphore.Release();
        }
    }

    private async Task LoadRepoDetailAsync(
        string solutionDir, string? pat, CancellationToken cancellationToken)
    {
        RepoDetailVisibility = "Visible";
        RepoListVisibility = "Collapsed";
        IssuesStatusMessage = "Loading issues...";
        PullRequestStatusMessage = "Loading pull requests...";

        var repoInfo = _repoDetector.DetectRepository(solutionDir);
        if (repoInfo == null)
        {
            StatusMessage = "No GitHub repository detected.";
            IssuesStatusMessage = string.Empty;
            PullRequestStatusMessage = string.Empty;
            RepoDetailVisibility = "Collapsed";
            RepositoryName = string.Empty;
            return;
        }

        RepositoryName = $"{repoInfo.Owner}/{repoInfo.RepositoryName}";

        var result = await _gitHubService.GetOpenItemsAsync(
            repoInfo.Owner, repoInfo.RepositoryName, pat, cancellationToken);

        foreach (var issue in result.Issues)
        {
            Issues.Add(issue);
        }

        foreach (var pr in result.PullRequests)
        {
            PullRequests.Add(pr);
        }

        IssuesStatusMessage = result.Issues.Count == 0
            ? "No open issues."
            : $"{result.Issues.Count} open issue{(result.Issues.Count == 1 ? "" : "s")}.";

        PullRequestStatusMessage = result.PullRequests.Count == 0
            ? "No open pull requests."
            : $"{result.PullRequests.Count} open pull request{(result.PullRequests.Count == 1 ? "" : "s")}.";
    }

    private async Task LoadUserRepositoriesAsync(
        string pat, CancellationToken cancellationToken)
    {
        RepoListVisibility = "Visible";
        RepoDetailVisibility = "Collapsed";
        RepositoryName = "Your Repositories";
        RepositoriesStatusMessage = "Loading repositories...";

        var repos = await _gitHubService.GetUserRepositoriesAsync(pat, cancellationToken);

        foreach (var repo in repos)
        {
            Repositories.Add(repo);
        }

        RepositoriesStatusMessage = repos.Count == 0
            ? "No repositories with open items."
            : $"{repos.Count} repositor{(repos.Count == 1 ? "y" : "ies")} with open items.";
    }

    private async Task<string?> GetSolutionDirectoryAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _extensibility.Workspaces().QuerySolutionAsync(
                solution => solution.With(s => s.Path),
                cancellationToken);

            var solutionSnapshot = result.FirstOrDefault();
            var solutionPath = solutionSnapshot?.Path;
            return solutionPath != null ? Path.GetDirectoryName(solutionPath) : null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> ReadPatAsync(CancellationToken cancellationToken)
    {
        try
        {
            var patResult = await _extensibility.Settings().ReadEffectiveValueAsync(
                GitInfoSettings.PersonalAccessToken, cancellationToken);
            var pat = patResult.Value;
            return string.IsNullOrWhiteSpace(pat) ? null : pat;
        }
        catch
        {
            return null;
        }
    }

    public void StartAutoRefresh()
    {
        _autoRefreshCts = new CancellationTokenSource();
        _ = Task.Run(() => AutoRefreshLoopAsync(_autoRefreshCts.Token));
        _ = Task.Run(() => SolutionMonitorLoopAsync(_autoRefreshCts.Token));
    }

    public void StopAutoRefresh()
    {
        _autoRefreshCts?.Cancel();
        _autoRefreshCts?.Dispose();
        _autoRefreshCts = null;
    }

    private async Task SolutionMonitorLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var timer = new PeriodicTimer(
                    TimeSpan.FromSeconds(SolutionMonitorIntervalSeconds));
                if (!await timer.WaitForNextTickAsync(cancellationToken))
                    break;

                var currentPath = await GetSolutionDirectoryAsync(cancellationToken);
                if (!string.Equals(currentPath, _lastKnownSolutionPath,
                        StringComparison.OrdinalIgnoreCase))
                {
                    // Don't update _lastKnownSolutionPath here; LoadDataAsync
                    // sets it after acquiring the semaphore. If the semaphore
                    // is held, the path stays stale so the next tick retries.
                    await LoadDataAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException) { break; }
            catch { /* keep loop alive */ }
        }
    }

    private async Task AutoRefreshLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var interval = await ReadRefreshIntervalAsync(cancellationToken);

            if (interval <= 0)
            {
                // Auto-refresh disabled; re-check the setting periodically
                using var waitTimer = new PeriodicTimer(TimeSpan.FromSeconds(60));
                if (!await waitTimer.WaitForNextTickAsync(cancellationToken))
                {
                    break;
                }

                continue;
            }

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(interval));
            if (!await timer.WaitForNextTickAsync(cancellationToken))
            {
                break;
            }

            await LoadDataAsync(cancellationToken);
        }
    }

    private async Task<int> ReadRefreshIntervalAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _extensibility.Settings().ReadEffectiveValueAsync(
                GitInfoSettings.RefreshInterval, cancellationToken);
            return int.TryParse(result.Value, out var seconds) ? seconds : 120;
        }
        catch
        {
            return 120;
        }
    }
}
