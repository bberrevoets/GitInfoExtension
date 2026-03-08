# GitInfoExtension

A Visual Studio 2026 extension that displays open issues and pull requests
for repositories connected to git hosting services (currently GitHub).

## Overview

This extension integrates with the GitHub API to provide an in-IDE view of:

- Open issues for the connected repository
- Open pull requests for the connected repository
- A summary of all your repositories with open items (when no solution is open)

## Features

- **Automatic repository detection** -- parses `.git/config` to find the
  GitHub remote for the current solution
- **Issues and pull requests** -- displays open items with title, author,
  labels, and branch info
- **Repository list** -- when no solution is open but a PAT is configured,
  shows all your repositories that have open items
- **Configurable auto-refresh** -- refresh interval adjustable from 1 to 30
  minutes (or disabled) via Tools > Options
- **Solution monitoring** -- detects solution open, close, and switch events
  within ~3 seconds and refreshes automatically
- **Manual refresh** -- one-click Refresh button in the tool window header
- **Extension icon** -- custom icon and preview image shown in the
  Extensions Manager

## Getting Started

### Prerequisites

- Visual Studio 2026 Preview or later (internal version 17.14+)
- .NET 8.0 SDK

> **Note:** Visual Studio 2022 (17.0--17.12) is **not compatible** -- the
> extension targets `[17.14,)` via the VS Extensibility SDK.

### Build

```bash
# Debug build
dotnet build GitInfoExtension/GitInfoExtension.csproj

# Release build (recommended for installation)
dotnet build GitInfoExtension/GitInfoExtension.csproj -c Release
```

### Installation

#### Option A: Double-click the VSIX (simplest)

1. Build in Release mode (see above)
2. Navigate to `GitInfoExtension\bin\Release\net8.0-windows8.0\`
3. Double-click `GitInfoExtension.vsix`
4. The VSIX Installer launches -- select your VS2026 instance and click
   **Install**

> **Note:** The VSIX Installer may list VS2022 as an available target, but
> installation will be blocked because the extension requires VS2026 (17.14+).
> Only VS2026 instances are valid targets.

1. Restart VS2026

#### Option B: F5 debugging (development)

1. Open `GitInfoExtension.slnx` in VS2026
2. Set the `GitInfoExtension` project as the startup project
3. Press **F5** -- VS launches an experimental instance with the extension
   loaded (no permanent install needed)
4. The extension is active only in the experimental instance

### Uninstalling

1. Clear your PAT in **Tools > Options > Git Info Extension** before
   uninstalling (the extension cannot clear settings automatically)
2. In VS2026, go to **Extensions > Manage Extensions**
3. Find **Git Info Extension** and click **Uninstall**
4. Restart VS2026

### Configuration

1. Open **Tools > Options > Git Info Extension**
2. Enter your **GitHub Personal Access Token** (required for private
   repositories and higher rate limits)
3. Set the **Auto-Refresh Interval** (default: 2 minutes)

> **Note:** After first install, a Visual Studio restart may be needed for the
> settings page to appear under Tools > Options.

### Usage

Open the tool window via **Extensions > Git Info**. When a solution with a
GitHub remote is loaded, the window displays open issues and pull requests.
When no solution is open but a PAT is configured, it shows a summary of your
repositories with open items.

## Testing

After installing the extension, verify the following:

- [ ] Extension appears in **Extensions > Manage Extensions**
- [ ] **Extensions > Git Info** menu item is present
- [ ] Opening a solution with a GitHub remote shows issues and PRs
- [ ] Closing all solutions updates the tool window within ~3 seconds
- [ ] PAT configuration in **Tools > Options** works correctly
- [ ] Auto-refresh fires at the configured interval
- [ ] Solution switch detection updates the displayed repository
- [ ] Manual **Refresh** button reloads data immediately
- [ ] UI respects the current VS theme (dark/light/blue)

## Versioning

This project uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)
for automatic version computation from git history. No manual version bumps
are needed for patch releases.

- **Development** builds: `1.0.X-beta` (pre-release)
- **main** builds: `1.0.X` (stable)
- To bump minor/major: edit `"version"` in `version.json`

## CI/CD

| Workflow | Trigger | What it does |
| --- | --- | --- |
| **CI** (`ci.yml`) | Push/PR to `Development` | Build + upload VSIX artifact |
| **Release** (`release.yml`) | Push to `main` or manual | Build, git tag, GitHub Release, VS Marketplace publish |

The release workflow publishes to the
[Visual Studio Marketplace](https://marketplace.visualstudio.com/) using the
`VS_MARKETPLACE_PAT` secret (Azure DevOps PAT with Marketplace > Manage scope).

## Technologies

- Visual Studio 2026 Extensibility SDK (`VisualStudio.Extensibility`)
- Nerdbank.GitVersioning (automatic version computation)
- GitHub API client (Octokit.net 13.x)
- C# / .NET 8.0

## VS Marketplace

The Marketplace overview page uses `MARKETPLACE.md` (user-facing) instead
of this README. The publish manifest (`publishManifest.json`) references
that file. Keep `MARKETPLACE.md` focused on end-user information
(features, installation, configuration, usage) and this README focused on
development.

## Branch Structure

- `Development` -- default branch for active development
- `main` -- production branch

## Author

Bert Berrevoets -- Berrevoets Systems
<bert@berrevoets.net>
