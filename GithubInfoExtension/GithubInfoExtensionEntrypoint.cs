namespace GithubInfoExtension;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

[VisualStudioContribution]
internal class GithubInfoExtensionEntrypoint : Extension
{
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
            id: "GithubInfoExtension.a1f3b2c4-5d6e-7f8a-9b0c-1d2e3f4a5b6c",
            version: ExtensionAssemblyVersion,
            publisherName: "Berrevoets Systems",
            displayName: "GitHub Info Extension",
            description: "Displays open GitHub issues and pull requests for the current repository.")
    };

    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);
    }
}
