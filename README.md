# GithubInfoExtension

A Visual Studio 2026 extension that displays open GitHub issues and pull
requests when the solution is connected to a GitHub repository.

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

## Getting Started

### Prerequisites

- Visual Studio 2026 Preview or later (internal version 17.14+)
- .NET 8.0 SDK

> **Note:** Visual Studio 2022 (17.0–17.12) is **not compatible** — the
> extension targets `[17.14,)` via the VS Extensibility SDK.

### Build

```bash
# Debug build
dotnet build GithubInfoExtension/GithubInfoExtension.csproj

# Release build (recommended for installation)
dotnet build GithubInfoExtension/GithubInfoExtension.csproj -c Release
```

### Installation

#### Option A: Double-click the VSIX (simplest)

1. Build in Release mode (see above)
2. Navigate to `GithubInfoExtension\bin\Release\net8.0-windows8.0\`
3. Double-click `GithubInfoExtension.vsix`
4. The VSIX Installer launches — select your VS2026 instance and click
   **Install**
5. Restart VS2026

#### Option B: F5 debugging (development)

1. Open `GithubInfoExtension.slnx` in VS2026
2. Set the `GithubInfoExtension` project as the startup project
3. Press **F5** — VS launches an experimental instance with the extension
   loaded (no permanent install needed)
4. The extension is active only in the experimental instance

### Uninstalling

1. In VS2026, go to **Extensions > Manage Extensions**
2. Find **GitHub Info Extension** and click **Uninstall**
3. Restart VS2026

### Configuration

1. Open **Tools > Options > GitHub Info Extension**
2. Enter your **GitHub Personal Access Token** (required for private
   repositories and higher rate limits)
3. Set the **Auto-Refresh Interval** (default: 2 minutes)

## Testing

After installing the extension, verify the following:

- [ ] Extension appears in **Extensions > Manage Extensions**
- [ ] **View > Other Windows > GitHub Info** menu item is present
- [ ] Opening a solution with a GitHub remote shows issues and PRs
- [ ] Closing all solutions updates the tool window within ~3 seconds
- [ ] PAT configuration in **Tools > Options** works correctly
- [ ] Auto-refresh fires at the configured interval
- [ ] Solution switch detection updates the displayed repository
- [ ] Manual **Refresh** button reloads data immediately
- [ ] UI respects the current VS theme (dark/light/blue)

## Technologies

- Visual Studio 2026 Extensibility SDK (`VisualStudio.Extensibility`)
- GitHub API client (Octokit.net 13.x)
- C# / .NET 8.0

## Branch Structure

- `Development` -- default branch for active development
- `main` -- production branch

## Author

Bert Berrevoets -- Berrevoets Systems
<bert@berrevoets.net>
