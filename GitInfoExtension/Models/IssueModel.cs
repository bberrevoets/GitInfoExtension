namespace GitInfoExtension.Models;

using System.Runtime.Serialization;
using Microsoft.VisualStudio.Extensibility.UI;

[DataContract]
internal class IssueModel : NotifyPropertyChangedObject
{
    private string _displayNumber = string.Empty;
    private string _title = string.Empty;
    private string _displayInfo = string.Empty;
    private string _labels = string.Empty;
    private string _url = string.Empty;

    [DataMember]
    public string DisplayNumber
    {
        get => _displayNumber;
        set => SetProperty(ref _displayNumber, value);
    }

    [DataMember]
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    [DataMember]
    public string DisplayInfo
    {
        get => _displayInfo;
        set => SetProperty(ref _displayInfo, value);
    }

    [DataMember]
    public string Labels
    {
        get => _labels;
        set => SetProperty(ref _labels, value);
    }

    [DataMember]
    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }
}
