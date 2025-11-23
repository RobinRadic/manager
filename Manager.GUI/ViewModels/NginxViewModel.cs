using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.Core.Features.Nginx;

namespace Manager.GUI.ViewModels;

public partial class NginxViewModel : ViewModelBase
{
    private readonly NginxService _nginxService;

    [ObservableProperty]
    private ObservableCollection<NginxSite> _sites = new();

    [ObservableProperty]
    private NginxSite? _selectedSite;

    [ObservableProperty]
    private string _currentConfigText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    public NginxViewModel(NginxService nginxService)
    {
        _nginxService = nginxService;
        LoadSitesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadSites()
    {
        StatusMessage = "Loading sites...";
        var sites = await _nginxService.LoadSitesAsync();
        Sites.Clear();
        foreach (var site in sites) Sites.Add(site);
        StatusMessage = $"{sites.Count} sites loaded.";
    }

    // Triggered when the user changes selection in the list
    partial void OnSelectedSiteChanged(NginxSite? value)
    {
        if (value != null)
        {
            LoadConfigForSite(value);
        }
        else
        {
            CurrentConfigText = "";
        }
    }

    private async void LoadConfigForSite(NginxSite site)
    {
        try
        {
            StatusMessage = $"Loading config for {site.Name}...";
            CurrentConfigText = await _nginxService.GetConfigAsync(site.Name);
            StatusMessage = $"Loaded {site.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading config: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveConfig()
    {
        if (SelectedSite == null) return;

        try
        {
            StatusMessage = "Saving and validating...";
            await _nginxService.SaveConfigAsync(SelectedSite.Name, CurrentConfigText);
            StatusMessage = "Configuration saved & Nginx reloaded successfully.";
        }
        catch (Exception ex)
        {
            // In a real app, use a Dialog Service here to show the full Nginx error
            StatusMessage = $"SAVE FAILED: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ToggleSite(NginxSite site)
    {
        if (site == null) return;
        
        bool newState = !site.IsEnabled;
        try
        {
            await _nginxService.ToggleSiteAsync(site, newState);
            
            // Update UI state locally to reflect change
            site.IsEnabled = newState;
            // Force refresh of the list item (trick to update UI icon)
            var index = Sites.IndexOf(site);
            if (index != -1) 
            {
                Sites[index] = new NginxSite 
                { 
                    Name = site.Name, 
                    FilePath = site.FilePath, 
                    IsEnabled = newState 
                };
            }
            
            StatusMessage = newState ? $"Enabled {site.Name}" : $"Disabled {site.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error toggling site: {ex.Message}";
        }
    }
}