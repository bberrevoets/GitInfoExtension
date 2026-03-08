# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added

- Extension icon (`Images/Icon.png`, 128x128) and preview image
  (`Images/PreviewImage.png`, 200x200) displayed in the VS2026
  Extensions Manager
- Icon and PreviewImage metadata references in extension entrypoint
- HTML-formatted extension description with features, getting started
  steps, and author credit in Extensions Manager
- `MoreInfo` URL and search tags in extension metadata
- VS2022 installer compatibility note in README.md
- PAT cleanup step in uninstall instructions
- Usage section in README.md explaining menu location and tool window
- First-install restart note for settings page visibility
- TESTREPORT.md with initial testing results and feedback
- Installation and testing documentation in README.md with step-by-step
  instructions for VSIX install, F5 debugging, and uninstalling
- VS version compatibility note (requires VS2026 / 17.14+, not VS2022)
- Test checklist covering activation, repo detection, PAT config,
  auto-refresh, solution monitoring, manual refresh, and UI theming
- Installation section in CLAUDE.md for development reference
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

- Excluded `CLAUDE.md` and `TESTREPORT.md` from version control via
  `.gitignore` (these files are developer-local only)
- Moved tool window command from View > Other Windows to Extensions menu
- Improved no-PAT status message with settings location and restart hint
- Updated footer hint in tool window XAML with restart note
- Repository branch structure: `Development` as default branch, `main` as
  production branch
- GitHub rulesets to protect `Development` and `main` branches
- Updated README.md with features, build instructions, and configuration

## [0.0.1] - 2026-03-07

### Added

- Initial VS2026 extension scaffold with Hello World tool window
- Extension entrypoint, command, tool window, and XAML content
- Initial README documenting the VS2026 extension concept
