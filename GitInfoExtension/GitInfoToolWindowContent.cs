namespace GitInfoExtension;

using Microsoft.VisualStudio.Extensibility.UI;

internal class GitInfoToolWindowContent : RemoteUserControl
{
    public GitInfoToolWindowContent(GitInfoToolWindowViewModel viewModel)
        : base(dataContext: viewModel)
    {
    }
}
