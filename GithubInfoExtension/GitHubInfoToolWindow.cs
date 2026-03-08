namespace GithubInfoExtension;

using GithubInfoExtension.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

[VisualStudioContribution]
internal class GitHubInfoToolWindow : ToolWindow
{
    private readonly GitHubInfoToolWindowViewModel _viewModel;

    public GitHubInfoToolWindow(
        VisualStudioExtensibility extensibility,
        IGitRepositoryDetector repoDetector,
        IGitHubService gitHubService)
        : base(extensibility)
    {
        Title = "GitHub Info";
        _viewModel = new GitHubInfoToolWindowViewModel(extensibility, repoDetector, gitHubService);
    }

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell,
    };

    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await base.InitializeAsync(cancellationToken);
        await _viewModel.LoadDataAsync(cancellationToken);
        _viewModel.StartAutoRefresh();
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _viewModel.StopAutoRefresh();
        }

        base.Dispose(isDisposing);
    }

    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(new GitHubInfoToolWindowContent(_viewModel));
    }
}
