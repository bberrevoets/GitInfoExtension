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

- Visual Studio 2026 Preview or later
- .NET 8.0 SDK

### Build

```bash
dotnet build GithubInfoExtension/GithubInfoExtension.csproj
```

### Configuration

1. Open **Tools > Options > GitHub Info Extension**
2. Enter your **GitHub Personal Access Token** (required for private
   repositories and higher rate limits)
3. Set the **Auto-Refresh Interval** (default: 2 minutes)

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
