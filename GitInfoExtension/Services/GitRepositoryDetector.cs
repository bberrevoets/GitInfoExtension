namespace GitInfoExtension.Services;

using System.IO;
using System.Text.RegularExpressions;
using GitInfoExtension.Models;

internal class GitRepositoryDetector : IGitRepositoryDetector
{
    private static readonly Regex GitHubUrlRegex = new(
        @"github\.com[:/](?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?$",
        RegexOptions.Compiled);

    private static readonly Regex AzDoModernUrlRegex = new(
        @"dev\.azure\.com/(?<org>[^/]+)/(?<project>[^/]+)/_git/(?<repo>[^/]+?)(?:\.git)?$",
        RegexOptions.Compiled);

    private static readonly Regex AzDoLegacyUrlRegex = new(
        @"(?<org>[^.]+)\.visualstudio\.com/(?<project>[^/]+)/_git/(?<repo>[^/]+?)(?:\.git)?$",
        RegexOptions.Compiled);

    private static readonly Regex AzDoSshUrlRegex = new(
        @"ssh\.dev\.azure\.com:v3/(?<org>[^/]+)/(?<project>[^/]+)/(?<repo>[^/]+?)(?:\.git)?$",
        RegexOptions.Compiled);

    public RepoInfo? DetectRepository(string solutionDirectory)
    {
        var gitDir = FindGitDirectory(solutionDirectory);
        if (gitDir == null)
            return null;

        var configPath = Path.Combine(gitDir, "config");
        if (!File.Exists(configPath))
            return null;

        return ParseGitConfig(File.ReadAllText(configPath));
    }

    private static string? FindGitDirectory(string directory)
    {
        var current = new DirectoryInfo(directory);
        while (current != null)
        {
            var gitPath = Path.Combine(current.FullName, ".git");

            if (Directory.Exists(gitPath))
                return gitPath;

            // Handle .git file (worktrees/submodules)
            if (File.Exists(gitPath))
            {
                var content = File.ReadAllText(gitPath).Trim();
                if (content.StartsWith("gitdir: "))
                {
                    var resolved = content.Substring("gitdir: ".Length);
                    if (!Path.IsPathRooted(resolved))
                        resolved = Path.GetFullPath(Path.Combine(current.FullName, resolved));
                    if (Directory.Exists(resolved))
                        return resolved;
                }
            }

            current = current.Parent;
        }

        return null;
    }

    internal static RepoInfo? ParseGitConfig(string configContent)
    {
        bool inRemoteOrigin = false;
        foreach (var line in configContent.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed == "[remote \"origin\"]")
            {
                inRemoteOrigin = true;
                continue;
            }

            if (trimmed.StartsWith('['))
            {
                inRemoteOrigin = false;
                continue;
            }

            if (inRemoteOrigin && trimmed.StartsWith("url = "))
            {
                var url = trimmed.Substring("url = ".Length).Trim();

                // Try GitHub first (preserves existing behavior)
                var githubMatch = GitHubUrlRegex.Match(url);
                if (githubMatch.Success)
                {
                    return new RepoInfo(
                        githubMatch.Groups["owner"].Value,
                        githubMatch.Groups["repo"].Value);
                }

                // Try Azure DevOps modern URL
                var azDoModernMatch = AzDoModernUrlRegex.Match(url);
                if (azDoModernMatch.Success)
                {
                    return new RepoInfo(
                        azDoModernMatch.Groups["org"].Value,
                        azDoModernMatch.Groups["repo"].Value,
                        GitProvider.AzureDevOps,
                        azDoModernMatch.Groups["project"].Value);
                }

                // Try Azure DevOps legacy URL
                var azDoLegacyMatch = AzDoLegacyUrlRegex.Match(url);
                if (azDoLegacyMatch.Success)
                {
                    return new RepoInfo(
                        azDoLegacyMatch.Groups["org"].Value,
                        azDoLegacyMatch.Groups["repo"].Value,
                        GitProvider.AzureDevOps,
                        azDoLegacyMatch.Groups["project"].Value);
                }

                // Try Azure DevOps SSH URL
                var azDoSshMatch = AzDoSshUrlRegex.Match(url);
                if (azDoSshMatch.Success)
                {
                    return new RepoInfo(
                        azDoSshMatch.Groups["org"].Value,
                        azDoSshMatch.Groups["repo"].Value,
                        GitProvider.AzureDevOps,
                        azDoSshMatch.Groups["project"].Value);
                }
            }
        }

        return null;
    }
}
