# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with
code in this repository.

## Project Overview

A Visual Studio 2026 extension that integrates with git hosting services
(currently GitHub) to display open issues and pull requests for the currently
connected repository within the IDE.

- **Type**: Visual Studio 2026 (VS2026) VSIX extension
- **Language**: C# (.NET)
- **GitHub repo**: `bberrevoets/GitInfoExtension`

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
- `Development` builds produce `1.0.X-beta` (pre-release)
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
| `ci.yml` | Push/PR to `Development` | Build, upload VSIX artifact |
| `release.yml` | Push to `main`, manual | Build, tag, GitHub Release, publish to VS Marketplace |

### VS Marketplace publishing

- **Publisher**: `BerrevoetsSystems`
- **Manifest**: `publishManifest.json`
- **Overview**: `MARKETPLACE.md` (user-facing; `README.md` is
  developer-focused and not shown on the Marketplace)
- Requires `VS_MARKETPLACE_PAT` GitHub Actions secret (Azure DevOps PAT
  with Marketplace > Manage scope)

## Key Technologies

- VS2026 Extensibility SDK (`Microsoft.VisualStudio.Extensibility.Sdk` 17.14)
- Nerdbank.GitVersioning 3.7.x (automatic version computation)
- GitHub API client (Octokit.net 13.x)
- .NET 8.0 (Windows)
