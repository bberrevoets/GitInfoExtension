# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Fixed

- Release builds on `main` no longer produce `-beta` suffix in tags,
  GitHub Release names, and assembly metadata. The previous "Set stable
  version" step was ineffective because nbgv reads from committed git
  state, not the working tree. Replaced with `SimpleVersion`-based
  release naming and a `StripBetaForRelease` MSBuild target activated
  by `/p:StripPrerelease=true`.
- Fix `StripBetaForRelease` build failure (`AssemblyVersion` empty):
  replaced `$(SimpleVersion)` (not available as MSBuild property) with
  `$(MajorMinorVersion).$(GitVersionHeight)` and explicitly set
  `AssemblyVersion` to prevent .NET SDK `GenerateDepsFile` error.

## [1.0.0] - 2026-03-08

### Added

- `MARKETPLACE.md` -- user-facing overview for the VS Marketplace page,
  separate from the developer-focused README
- Automatic versioning via Nerdbank.GitVersioning (`version.json`)
  - `Development` builds: `1.0.X-beta` (pre-release)
  - `main` builds: `1.0.X` (stable)
- CI workflow (`.github/workflows/ci.yml`) -- builds on push/PR to
  `Development`, uploads VSIX artifact
- Release workflow (`.github/workflows/release.yml`) -- builds on push to
  `main` or manual trigger, creates git tag, GitHub Release with VSIX,
  and publishes to VS Marketplace
- VS Marketplace publish manifest (`publishManifest.json`)
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
- GitHub API integration via Octokit.net for fetching issues and pull
  requests
- `GitRepositoryDetector` service to parse `.git/config` and detect
  GitHub remote owner/repo
- `GitHubService` for fetching open issues, pull requests, and user
  repository summaries
- Data models: `IssueModel`, `PullRequestModel`, `RepoInfo`,
  `ItemsResult`, `RepositorySummaryModel`
- `GitInfoToolWindowViewModel` with full data-binding for issues,
  PRs, and repository lists
- WPF tool window UI (`GitInfoToolWindowContent.xaml`) with styled
  lists for issues, pull requests, and repository summaries
- Configurable settings (PAT and auto-refresh interval) via
  Tools > Options > Git Info Extension
- Auto-refresh background loop with configurable interval
- Solution monitor loop (3-second poll) to detect solution open, close,
  and switch events and trigger immediate refresh
- Concurrency guard (`SemaphoreSlim`) to prevent overlapping data loads
- DI registration for services in `GitInfoExtensionEntrypoint`
- CLAUDE.md with project guidance for Claude Code
- CHANGELOG.md

### Changed

- `publishManifest.json` now points `overview` to `MARKETPLACE.md`
  instead of `README.md`
- Renamed extension from "GitHub Info" to "Git Info" to prepare for
  multi-provider support (e.g. Azure DevOps)
- Renamed project directory `GithubInfoExtension` to `GitInfoExtension`
- Renamed solution file to `GitInfoExtension.slnx`
- Renamed model classes: `GitHubIssueModel` to `IssueModel`,
  `GitHubPullRequestModel` to `PullRequestModel`,
  `GitHubRepositorySummaryModel` to `RepositorySummaryModel`,
  `GitHubItemsResult` to `ItemsResult`, `GitHubRepoInfo` to `RepoInfo`
- Renamed settings class from `GitHubSettings` to `GitInfoSettings`
- Renamed tool window classes: `GitHubInfoToolWindow` to `GitInfoToolWindow`,
  `GitHubInfoToolWindowCommand` to `GitInfoToolWindowCommand`,
  `GitHubInfoToolWindowContent` to `GitInfoToolWindowContent`,
  `GitHubInfoToolWindowViewModel` to `GitInfoToolWindowViewModel`
- Updated namespace from `GithubInfoExtension` to `GitInfoExtension`
- Updated all display names from "GitHub Info" to "Git Info"
- Settings category ID changed from `githubInfoExtension` to
  `gitInfoExtension` (users will need to re-enter their PAT after upgrade)
- Menu location changed to **Extensions > Git Info**
- Settings location changed to **Tools > Options > Git Info Extension**

### Removed

- Old `GithubInfoExtension` project directory and namespace (replaced
  by `GitInfoExtension`)

## [0.0.1] - 2026-03-07

### Added

- Initial VS2026 extension scaffold with Hello World tool window
- Extension entrypoint, command, tool window, and XAML content
- Initial README documenting the VS2026 extension concept
