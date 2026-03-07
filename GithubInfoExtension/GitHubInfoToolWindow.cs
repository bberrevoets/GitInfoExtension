namespace GithubInfoExtension;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

[VisualStudioContribution]
internal class GitHubInfoToolWindow : ToolWindow
{
    public GitHubInfoToolWindow(VisualStudioExtensibility extensibility)
        : base(extensibility)
    {
        Title = "GitHub Info";
    }

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell,
    };

    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(new GitHubInfoToolWindowContent());
    }
}
