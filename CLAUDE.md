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

This project targets Visual Studio 2026 extensibility. No solution or project
files exist yet. Update this section once the project is scaffolded with actual
build, test, and package commands.

## Architecture Notes

The extension should:

- Detect when a VS2026 solution is connected to a GitHub repository
- Use the GitHub API (via Octokit.net or GitHub REST/GraphQL API) to fetch
  open issues and pull requests
- Display this information in a VS2026 tool window

## Key Technologies

- VS2026 Extensibility SDK (VisualStudio.Extensibility)
- GitHub API client (Octokit.net recommended)
- .NET (version TBD based on VS2026 extensibility requirements)
