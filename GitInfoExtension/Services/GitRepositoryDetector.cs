namespace GitInfoExtension.Services;

using System.IO;
using System.Text.RegularExpressions;
using GitInfoExtension.Models;

internal class GitRepositoryDetector : IGitRepositoryDetector
{
    private static readonly Regex GitHubUrlRegex = new(
        @"github\.com[:/](?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?$",
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
                var match = GitHubUrlRegex.Match(url);
                if (match.Success)
                {
                    return new RepoInfo(
                        match.Groups["owner"].Value,
                        match.Groups["repo"].Value);
                }
            }
        }

        return null;
    }
}
