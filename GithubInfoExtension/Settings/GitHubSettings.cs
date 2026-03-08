namespace GithubInfoExtension.Settings;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;

internal static class GitHubSettings
{
    [VisualStudioContribution]
    internal static SettingCategory GitHubInfoCategory { get; } =
        new("githubInfoExtension", "%GitHubSettings.CategoryTitle%")
        {
            Description = "%GitHubSettings.CategoryDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.String PersonalAccessToken { get; } =
        new("personalAccessToken", "%GitHubSettings.PatTitle%", GitHubInfoCategory, defaultValue: string.Empty)
        {
            Description = "%GitHubSettings.PatDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.Enum RefreshInterval { get; } =
        new("refreshInterval",
            "%GitHubSettings.RefreshIntervalTitle%",
            GitHubInfoCategory,
            [
                new("0", "%GitHubSettings.RefreshInterval.Disabled%"),
                new("60", "%GitHubSettings.RefreshInterval.1Min%"),
                new("120", "%GitHubSettings.RefreshInterval.2Min%"),
                new("300", "%GitHubSettings.RefreshInterval.5Min%"),
                new("600", "%GitHubSettings.RefreshInterval.10Min%"),
                new("1800", "%GitHubSettings.RefreshInterval.30Min%"),
            ],
            defaultValue: "120")
        {
            Description = "%GitHubSettings.RefreshIntervalDescription%",
        };
}
