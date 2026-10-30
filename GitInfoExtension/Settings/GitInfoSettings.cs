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

    // Azure DevOps settings

    [VisualStudioContribution]
    internal static Setting.String AzureDevOpsPat { get; } =
        new("azureDevOpsPat", "%GitInfoSettings.AzureDevOps.PatTitle%", GitInfoCategory, defaultValue: string.Empty)
        {
            Description = "%GitInfoSettings.AzureDevOps.PatDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.String AzureDevOpsMainProject { get; } =
        new("azureDevOpsMainProject", "%GitInfoSettings.AzureDevOps.MainProjectTitle%", GitInfoCategory, defaultValue: string.Empty)
        {
            Description = "%GitInfoSettings.AzureDevOps.MainProjectDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.String AzureDevOpsTagFilter { get; } =
        new("azureDevOpsTagFilter", "%GitInfoSettings.AzureDevOps.TagFilterTitle%", GitInfoCategory, defaultValue: string.Empty)
        {
            Description = "%GitInfoSettings.AzureDevOps.TagFilterDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.String AzureDevOpsStates { get; } =
        new("azureDevOpsStates", "%GitInfoSettings.AzureDevOps.StatesTitle%", GitInfoCategory, defaultValue: "New,Active,Resolved")
        {
            Description = "%GitInfoSettings.AzureDevOps.StatesDescription%",
        };

    [VisualStudioContribution]
    internal static Setting.String AzureDevOpsAssignedTo { get; } =
        new("azureDevOpsAssignedTo", "%GitInfoSettings.AzureDevOps.AssignedToTitle%", GitInfoCategory, defaultValue: string.Empty)
        {
            Description = "%GitInfoSettings.AzureDevOps.AssignedToDescription%",
        };
}
