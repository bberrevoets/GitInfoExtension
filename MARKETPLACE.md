# Git Info Extension for Visual Studio 2026

View open issues and pull requests for your repository directly inside
Visual Studio.

## Features

- **Issues and pull requests** -- see open items for the currently loaded
  solution, including title, author, labels, and branch info
- **Repository list** -- when no solution is open, browse all your
  repositories that have open items
- **Automatic repository detection** -- the extension reads your
  `.git/config` to find the GitHub remote automatically
- **Auto-refresh** -- keeps data up to date with a configurable refresh
  interval (1--30 minutes)
- **Solution monitoring** -- detects solution open, close, and switch
  events and refreshes within seconds
- **Manual refresh** -- one-click Refresh button in the tool window header

## Requirements

- Visual Studio 2026 (17.14 or later)

> **Note:** This extension is **not compatible** with Visual Studio 2022.

## Installation

1. Search for **Git Info Extension** in the Visual Studio Marketplace or
   in **Extensions > Manage Extensions** inside VS2026
2. Click **Install** and restart Visual Studio

## Configuration

1. Open **Tools > Options > Git Info Extension**
2. Enter your **GitHub Personal Access Token** (PAT) -- required for
   private repositories and higher API rate limits
3. Set the **Auto-Refresh Interval** (default: 2 minutes, range: 1--30
   minutes, or disable)

> **Tip:** After the first install, you may need to restart Visual Studio
> for the settings page to appear.

## Usage

Open the tool window via **Extensions > Git Info**.

- **With a solution open:** displays open issues and pull requests for
  the connected GitHub repository
- **Without a solution:** shows a summary of your repositories with
  open items (requires a configured PAT)

## Author

Bert Berrevoets -- Berrevoets Systems
[bert@berrevoets.net](mailto:bert@berrevoets.net)
