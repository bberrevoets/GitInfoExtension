namespace GitInfoExtension;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

[VisualStudioContribution]
internal class GitInfoToolWindowCommand : Command
{
    public GitInfoToolWindowCommand()
    {
    }

    public override CommandConfiguration CommandConfiguration => new("%GitInfoExtension.GitInfoToolWindowCommand.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.GitRepository, IconSettings.IconAndText),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await Extensibility.Shell().ShowToolWindowAsync<GitInfoToolWindow>(activate: true, cancellationToken);
    }
}
