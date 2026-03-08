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
                + "currently loaded repository directly inside Visual Studio. "
                + "Automatic repo detection, configurable auto-refresh, "
                + "and solution switch detection.")
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
