using CommunityToolkit.Mvvm.ComponentModel;

namespace Manager.GUI.ViewModels;

public partial class HostsPreviewViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _content;

    public HostsPreviewViewModel(string content)
    {
        _content = content;
    }
}