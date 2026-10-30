namespace GitInfoExtension.Models;

internal enum GitProvider { GitHub, AzureDevOps }

internal record RepoInfo(
    string Owner,
    string RepositoryName,
    GitProvider Provider = GitProvider.GitHub,
    string? Project = null);
