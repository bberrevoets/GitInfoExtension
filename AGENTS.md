# AGENTS.md

Instructions for AI coding agents (Claude Code, Codex and others) working in this repository. `CLAUDE.md` only imports
this file, so edit this one. [README.md](README.md) describes the project for people; this file lists what the code
alone does not tell you.

<!-- repo-init:start -->
## Repository Info

- **Company:** Berrevoets Systems
- **Author:** Bert Berrevoets
- **Contact:** <bert@berrevoets.net>
- **Remote:** <https://github.com/bberrevoets/GitInfoExtension> (public)

## Workflow Rules

- **GitHub Flow:** `main` is the default and only long-lived branch. It is protected by the ruleset "Protect main":
  no direct pushes, no deletion, changes only through a pull request, and all PR conversations must be resolved
  before merging. Merged head branches are deleted automatically.
- After cloning, run `git config pull.rebase true` once, so `git pull` rebases instead of creating merge commits.
- Work on a branch per topic, push it, and open a PR to `main` only when Bert asks for it, assigned to him
  (`gh pr create --assignee bberrevoets`). PRs are squash-merged. After a merge: switch to `main` and pull, then delete the
  local branch with `git branch -D <branch>` (`-d` refuses after a squash merge) once
  `gh pr view <branch> --json state,headRefOid` shows `MERGED` and the same commit as `git rev-parse <branch>`.
- **"Update the documentation"** means `AGENTS.md`, the project's own `README.md` files (not vendored third-party ones)
  and `CHANGELOG.md`; then commit and push.
- [CHANGELOG.md](CHANGELOG.md) follows Keep a Changelog: the Attributors list at the top, and one `Author: *Name*`
  line under each section heading (`### Added`, `### Changed`, ...).
- Markdown must pass `markdownlint-cli2 "**/*.md"` with [.markdownlint.json](.markdownlint.json) (180-character
  lines) before every commit.
- Binaries go through Git LFS: every `filter=lfs` pattern in `.gitattributes` — the standard set (PDF, ZIP, PNG,
  JPEG, STEP/STP, STL, 3MF, F3D/F3Z) plus any pattern this repository adds. Add a pattern there before committing
  another binary type (GIF, BMP, ...). The patterns are lowercase and match `X.PNG` only where `core.ignorecase` is
  true (Windows, macOS); on a case-sensitive system check a new binary with `git check-attr filter -- <file>` before
  committing it.
- Commit messages use the conventional style (`feat:`, `fix:`, `docs:`, `chore:`).
<!-- repo-init:end -->

## Project Overview

A Visual Studio 2026 extension that integrates with git hosting services
(currently GitHub) to display open issues and pull requests for the currently
connected repository within the IDE.

- **Type**: Visual Studio 2026 (VS2026) VSIX extension
- **Language**: C# (.NET)
- **GitHub repo**: `bberrevoets/GitInfoExtension` (public, MIT licence)
- **Published**: on the VS Marketplace as "Git Info Extension" (publisher `BerrevoetsSystems`); v1.0.27 with 106
  installs on 2026-10-30. Changes reach real users.
- **In progress**: Azure DevOps support (draft PR #33, Bert's unfinished work of 2026-03-09).

## Build and Development

```bash
# Build the extension (produces .vsix)
dotnet build GitInfoExtension/GitInfoExtension.csproj

# Build in Release mode
dotnet build GitInfoExtension/GitInfoExtension.csproj -c Release
```

- **Solution file**: `GitInfoExtension.slnx`
- **Target framework**: `net8.0-windows8.0`
- **Output**: `GitInfoExtension.vsix` in `bin/Debug/` or `bin/Release/`

## Architecture Notes

The extension follows the VS2026 out-of-process extensibility model:

- **Entrypoint**: `GitInfoExtensionEntrypoint` registers DI services and
  defines extension metadata (HTML description, MoreInfo URL, tags)
- **Tool window**: `GitInfoToolWindow` creates the ViewModel and manages
  its lifecycle (init, auto-refresh, dispose)
- **ViewModel**: `GitInfoToolWindowViewModel` contains all business logic:
  - Queries VS for the current solution path
  - Delegates to `IGitRepositoryDetector` to find GitHub remote info
  - Delegates to `IGitHubService` to fetch issues/PRs via Octokit
  - Runs two background loops: auto-refresh (configurable interval) and
    solution monitor (3-second poll for solution open/close/switch)
  - Uses `SemaphoreSlim` concurrency guard to prevent overlapping loads
- **View**: `GitInfoToolWindowContent.xaml` (WPF DataTemplate) with
  data-bound lists for issues, PRs, and repository summaries
- **Settings**: `GitInfoSettings` exposes PAT and refresh interval via
  Tools > Options

### Key directories

| Directory | Contents |
| ----------- | ---------- |
| `Models/` | Data models (`IssueModel`, `PullRequestModel`, etc.) |
| `Services/` | `GitRepositoryDetector` (parses `.git/config`), `GitHubService` (Octokit wrapper) |
| `Settings/` | VS extensibility settings definition |

## Installation

The built `.vsix` targets VS2026 (internal version `[17.14,)`). It is
**not compatible** with VS2022.

- **VSIX install**: double-click
  `GitInfoExtension/bin/Release/net8.0-windows8.0/GitInfoExtension.vsix`
- **F5 debugging**: open `GitInfoExtension.slnx` in VS2026 and press F5
  to launch an experimental instance
- **Menu location**: Extensions > Git Info (opens the tool window)
- **Uninstall**: clear PAT in Tools > Options first, then
  Extensions > Manage Extensions > Uninstall > Restart VS

## Versioning and CI/CD

The project uses **Nerdbank.GitVersioning** (nbgv) for automatic version
computation from git history:

- **Config**: `version.json` at repo root (`"version": "1.0-beta"`)
- Builds outside `main` (pull request builds) produce `1.0.X-beta` (pre-release)
- `main` builds produce `1.0.X` (stable release)
- Release builds pass `/p:StripPrerelease=true` to strip the beta
  suffix from `Version`, `PackageVersion`, `AssemblyVersion`, and
  `InformationalVersion` via the `StripBetaForRelease` MSBuild target
  in the `.csproj` (uses `$(MajorMinorVersion).$(GitVersionHeight)`)
- The release workflow uses nbgv's `SimpleVersion` (3-part, no suffix)
  for tags and GitHub Release names on `main`
- Bump minor/major by editing the `"version"` field in `version.json`
- The entrypoint's `ExtensionAssemblyVersion` reads from the assembly,
  so the version flows through to the VSIX manifest automatically

### GitHub Actions workflows

| Workflow | Trigger | Purpose |
| --- | --- | --- |
| `ci.yml` | Push/PR to `main` | Build, upload VSIX artifact |
| `release.yml` | Push to `main`, manual | Build, tag, GitHub Release, publish to VS Marketplace |

**Every merge to `main` publishes a new Marketplace version**, documentation-only PRs included (Bert's choice,
2026-10-30, when the former `Development` branch was retired). Say so in a PR description when it matters.

### VS Marketplace publishing

- **Publisher**: `BerrevoetsSystems`
- **Manifest**: `publishManifest.json`
- **Overview**: `MARKETPLACE.md` (user-facing; `README.md` is
  developer-focused and not shown on the Marketplace)
- Requires `VS_MARKETPLACE_PAT` GitHub Actions secret (Azure DevOps PAT
  with Marketplace > Manage scope). The secret was set on 2026-03-08 and Azure DevOps PATs expire (one year at most):
  when the "Publish to VS Marketplace" step fails, renew the PAT first. The tag and GitHub Release are created before
  that step, so they exist even when the publish fails.

## Key Technologies

- VS2026 Extensibility SDK (`Microsoft.VisualStudio.Extensibility.Sdk` 17.14)
- Nerdbank.GitVersioning 3.7.x (automatic version computation)
- GitHub API client (Octokit.net 13.x)
- .NET 8.0 (Windows)

## Conventions

- `GitInfoExtension/Images/Icon.png` and `PreviewImage.png` (the Marketplace icon and preview, packed into the VSIX
  through `<Content Include>` and the entrypoint metadata) are **plain git blobs on purpose**: exception lines after
  the repo-init block in `.gitattributes`. The workflows' `actions/checkout` does not fetch LFS files, so in LFS the
  VSIX would ship pointer files as images.
