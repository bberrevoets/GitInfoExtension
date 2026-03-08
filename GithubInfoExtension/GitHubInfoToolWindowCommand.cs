namespace GithubInfoExtension;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

[VisualStudioContribution]
internal class GitHubInfoToolWindowCommand : Command
{
    public GitHubInfoToolWindowCommand()
    {
    }

    public override CommandConfiguration CommandConfiguration => new("%GithubInfoExtension.GitHubInfoToolWindowCommand.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ViewOtherWindowsMenu],
        Icon = new(ImageMoniker.KnownValues.GitRepository, IconSettings.IconAndText),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await Extensibility.Shell().ShowToolWindowAsync<GitHubInfoToolWindow>(activate: true, cancellationToken);
    }
}
