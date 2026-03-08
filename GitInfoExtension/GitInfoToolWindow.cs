namespace GitInfoExtension;

using GitInfoExtension.Services;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

[VisualStudioContribution]
internal class GitInfoToolWindow : ToolWindow
{
    private readonly GitInfoToolWindowViewModel _viewModel;

    public GitInfoToolWindow(
        VisualStudioExtensibility extensibility,
        IGitRepositoryDetector repoDetector,
        IGitHubService gitHubService)
        : base(extensibility)
    {
        Title = "Git Info";
        _viewModel = new GitInfoToolWindowViewModel(extensibility, repoDetector, gitHubService);
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
        return Task.FromResult<IRemoteUserControl>(new GitInfoToolWindowContent(_viewModel));
    }
}
