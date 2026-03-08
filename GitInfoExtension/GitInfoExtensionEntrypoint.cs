namespace GitInfoExtension;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

[VisualStudioContribution]
internal class GitInfoExtensionEntrypoint : Extension
{
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
            id: "GitInfoExtension.a1f3b2c4-5d6e-7f8a-9b0c-1d2e3f4a5b6c",
            version: ExtensionAssemblyVersion,
            publisherName: "Berrevoets Systems",
            displayName: "Git Info Extension",
            description: "View open issues and pull requests for the "
                + "currently loaded repository directly inside Visual Studio."
                + "<br/><br/>"
                + "<b>Features</b>"
                + "<ul>"
                + "<li>Automatic repository detection from .git/config</li>"
                + "<li>Open issues and pull requests with labels, authors, "
                + "and branch info</li>"
                + "<li>Repository overview when no solution is open</li>"
                + "<li>Configurable auto-refresh interval</li>"
                + "<li>Automatic solution switch detection</li>"
                + "</ul>"
                + "<b>Getting started</b>"
                + "<ol>"
                + "<li>Open <b>Extensions &gt; Git Info</b></li>"
                + "<li>Configure your Personal Access Token in "
                + "<b>Tools &gt; Options &gt; Git Info Extension</b></li>"
                + "</ol>"
                + "<br/>"
                + "<em>Developed by Bert Berrevoets &mdash; Berrevoets Systems</em>")
        {
            MoreInfo = "https://github.com/bberrevoets/GitInfoExtension",
            Tags = ["GitHub", "Git", "Issues", "Pull Requests", "Repository"],
            Icon = "Images/Icon.png",
            PreviewImage = "Images/PreviewImage.png",
        },
    };

    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);
        serviceCollection.AddSingleton<Services.IGitRepositoryDetector, Services.GitRepositoryDetector>();
        serviceCollection.AddSingleton<Services.IGitHubService, Services.GitHubService>();
    }
}
