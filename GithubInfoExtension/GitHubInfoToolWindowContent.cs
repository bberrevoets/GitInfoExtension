namespace GithubInfoExtension;

using Microsoft.VisualStudio.Extensibility.UI;

internal class GitHubInfoToolWindowContent : RemoteUserControl
{
    public GitHubInfoToolWindowContent(GitHubInfoToolWindowViewModel viewModel)
        : base(dataContext: viewModel)
    {
    }
}
