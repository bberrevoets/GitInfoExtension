# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added

- GitHub API integration via Octokit.net for fetching issues and pull requests
- `GitRepositoryDetector` service to parse `.git/config` and detect
  GitHub remote owner/repo
- `GitHubService` for fetching open issues, pull requests, and user
  repository summaries
- Data models: `GitHubIssueModel`, `GitHubPullRequestModel`,
  `GitHubRepoInfo`, `GitHubItemsResult`, `GitHubRepositorySummaryModel`
- `GitHubInfoToolWindowViewModel` with full data-binding for issues,
  PRs, and repository lists
- WPF tool window UI (`GitHubInfoToolWindowContent.xaml`) with styled
  lists for issues, pull requests, and repository summaries
- Configurable settings (PAT and auto-refresh interval) via
  Tools > Options > GitHub Info Extension
- Auto-refresh background loop with configurable interval
- Solution monitor loop (3-second poll) to detect solution open, close,
  and switch events and trigger immediate refresh
- Concurrency guard (`SemaphoreSlim`) to prevent overlapping data loads
- DI registration for services in `GithubInfoExtensionEntrypoint`
- CLAUDE.md with project guidance for Claude Code
- CHANGELOG.md

### Changed

- Repository branch structure: `Development` as default branch, `main` as
  production branch
- GitHub rulesets to protect `Development` and `main` branches
- Updated README.md with features, build instructions, and configuration

## [0.0.1] - 2026-03-07

### Added

- Initial VS2026 extension scaffold with Hello World tool window
- Extension entrypoint, command, tool window, and XAML content
- Initial README documenting the VS2026 extension concept
