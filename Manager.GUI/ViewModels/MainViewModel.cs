using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Manager.GUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // The DI Container injects these specific ViewModels
    private readonly HostsViewModel _hostsVm;
    private readonly NginxViewModel _nginxVm; 
    private readonly PhpViewModel _phpVm;
    private readonly SettingsViewModel _settingsVm; 
    // private readonly DatabaseViewModel _dbVm; // Uncomment later

    // This property determines what the user sees in the main window
    [ObservableProperty] private ViewModelBase _currentPage;

    [ObservableProperty] private string _paneTitle = "Dashboard";

    // Constructor Injection
    public MainViewModel(
        HostsViewModel hostsVm,
        NginxViewModel nginxVm,
        PhpViewModel phpVm,
        SettingsViewModel settingsVm
        )
    {
        _hostsVm = hostsVm;
        _nginxVm = nginxVm;
        _phpVm = phpVm;
        _settingsVm = settingsVm;

        // Default to Hosts view for now (or a Dashboard view)
        _currentPage = _hostsVm;
    }

    [RelayCommand]
    private void NavigateToHosts()
    {
        CurrentPage = _hostsVm;
        PaneTitle = "Hosts Manager";
    }

    [RelayCommand]
    private void NavigateToNginx()
    {
        CurrentPage = _nginxVm;
        PaneTitle = "Nginx Manager";
    }

    [RelayCommand]
    private void NavigateToPhp()
    {
        CurrentPage = _phpVm;
        PaneTitle = "PHP Manager";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentPage = _settingsVm;
        PaneTitle = "Settings";
    }
}