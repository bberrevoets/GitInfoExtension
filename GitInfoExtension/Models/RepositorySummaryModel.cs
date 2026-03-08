namespace GitInfoExtension.Models;

using System.Runtime.Serialization;
using Microsoft.VisualStudio.Extensibility.UI;

[DataContract]
internal class RepositorySummaryModel : NotifyPropertyChangedObject
{
    private string _fullName = string.Empty;
    private string _openItemsCount = string.Empty;
    private string _description = string.Empty;
    private string _url = string.Empty;

    [DataMember]
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    [DataMember]
    public string OpenItemsCount
    {
        get => _openItemsCount;
        set => SetProperty(ref _openItemsCount, value);
    }

    [DataMember]
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    [DataMember]
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }
}
