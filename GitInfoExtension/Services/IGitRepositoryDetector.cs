namespace GitInfoExtension.Services;

using GitInfoExtension.Models;

internal interface IGitRepositoryDetector
{
    RepoInfo? DetectRepository(string solutionDirectory);
}
