namespace GithubInfoExtension.Services;

using GithubInfoExtension.Models;

internal interface IGitRepositoryDetector
{
    GitHubRepoInfo? DetectRepository(string solutionDirectory);
}
