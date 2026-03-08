namespace GitInfoExtension.Settings;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;

internal static class GitInfoSettings
{
    [VisualStudioContribution]
    internal static SettingCategory GitInfoCategory { get; } =
        new("gitInfoExtension", "%GitInfoSettings.CategoryTitle%")
        {
            Description = "%GitInfoSettings.CategoryDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.String PersonalAccessToken { get; } =
        new("personalAccessToken", "%GitInfoSettings.PatTitle%", GitInfoCategory, defaultValue: string.Empty)
        {
            Description = "%GitInfoSettings.PatDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.Enum RefreshInterval { get; } =
        new("refreshInterval",
            "%GitInfoSettings.RefreshIntervalTitle%",
            GitInfoCategory,
            [
                new("0", "%GitInfoSettings.RefreshInterval.Disabled%"),
                new("60", "%GitInfoSettings.RefreshInterval.1Min%"),
                new("120", "%GitInfoSettings.RefreshInterval.2Min%"),
                new("300", "%GitInfoSettings.RefreshInterval.5Min%"),
                new("600", "%GitInfoSettings.RefreshInterval.10Min%"),
                new("1800", "%GitInfoSettings.RefreshInterval.30Min%"),
            ],
            defaultValue: "120")
        {
            Description = "%GitInfoSettings.RefreshIntervalDescription%",
        };
}
