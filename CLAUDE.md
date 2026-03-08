# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with
code in this repository.

## Project Overview

A Visual Studio 2026 extension that integrates with GitHub to display open
issues and pull requests for the currently connected repository within the IDE.

- **Type**: Visual Studio 2026 (VS2026) VSIX extension
- **Language**: C# (.NET)
- **GitHub repo**: `bberrevoets/GithubInfoExtension`

## Build and Development

```bash
# Build the extension (produces .vsix)
dotnet build GithubInfoExtension/GithubInfoExtension.csproj

# Build in Release mode
dotnet build GithubInfoExtension/GithubInfoExtension.csproj -c Release
```

- **Solution file**: `GithubInfoExtension.slnx`
- **Target framework**: `net8.0-windows8.0`
- **Output**: `GithubInfoExtension.vsix` in `bin/Debug/` or `bin/Release/`

## Architecture Notes

The extension follows the VS2026 out-of-process extensibility model:

- **Entrypoint**: `GithubInfoExtensionEntrypoint` registers DI services
- **Tool window**: `GitHubInfoToolWindow` creates the ViewModel and manages
  its lifecycle (init, auto-refresh, dispose)
- **ViewModel**: `GitHubInfoToolWindowViewModel` contains all business logic:
  - Queries VS for the current solution path
  - Delegates to `IGitRepositoryDetector` to find GitHub remote info
  - Delegates to `IGitHubService` to fetch issues/PRs via Octokit
  - Runs two background loops: auto-refresh (configurable interval) and
    solution monitor (3-second poll for solution open/close/switch)
  - Uses `SemaphoreSlim` concurrency guard to prevent overlapping loads
- **View**: `GitHubInfoToolWindowContent.xaml` (WPF DataTemplate) with
  data-bound lists for issues, PRs, and repository summaries
- **Settings**: `GitHubSettings` exposes PAT and refresh interval via
  Tools > Options

### Key directories

| Directory | Contents |
| ----------- | ---------- |
| `Models/` | Data models (`GitHubIssueModel`, `GitHubPullRequestModel`, etc.) |
| `Services/` | `GitRepositoryDetector` (parses `.git/config`), `GitHubService` (Octokit wrapper) |
| `Settings/` | VS extensibility settings definition |

## Key Technologies

- VS2026 Extensibility SDK (`Microsoft.VisualStudio.Extensibility.Sdk` 17.14)
- GitHub API client (Octokit.net 13.x)
- .NET 8.0 (Windows)
