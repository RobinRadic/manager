using CommunityToolkit.Mvvm.ComponentModel;

namespace Manager.GUI.Services.Nginx;

// Inherit from ObservableObject so the UI updates automatically when properties change
public partial class NginxSnippet : ObservableObject
{
    [ObservableProperty] 
    private string _name = "New Snippet";

    [ObservableProperty] 
    private string _content = "";

    [ObservableProperty] 
    private string _description = "";
}