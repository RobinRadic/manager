using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.Core.Features.Nginx;
using Manager.GUI.Services.Nginx;
using Manager.GUI.Services.Settings;

namespace Manager.GUI.ViewModels;

public partial class NginxViewModel : ViewModelBase
{
    private readonly NginxService _nginxService;
    private readonly SettingsService _settingsService;
    private readonly NginxSnippetService _snippetService; // New Dependency

    // Events
    public event Action<string>? InsertTextRequested; // View subscribes to this
    public event Action<NginxSnippet>? EditSnippetRequested; // View subscribes to this

    // Collections
    [ObservableProperty] private ObservableCollection<NginxSite> _sites = new();
    [ObservableProperty] private ObservableCollection<NginxSnippet> _snippets = new(); // New Collection

    // Properties
    [ObservableProperty] private NginxSite? _selectedSite;
    [ObservableProperty] private NginxSnippet? _selectedSnippet; // Selection for editing
    [ObservableProperty] private string _currentConfigText = string.Empty;
    [ObservableProperty] private string _statusMessage = "Ready";

    // Settings properties...
    [ObservableProperty] private ObservableCollection<string> _availableThemes = new();
    [ObservableProperty] private string _currentTheme = "dark_vs";
    [ObservableProperty] private FontFamily _editorFontFamily;
    [ObservableProperty] private double _editorFontSize;

    public NginxViewModel(
        NginxService nginxService,
        SettingsService settingsService,
        NginxSnippetService snippetService) // Inject
    {
        _nginxService = nginxService;
        _settingsService = settingsService;
        _snippetService = snippetService;

        _settingsService.SettingsChanged += OnSettingsChanged;
        ApplySettings(_settingsService.CurrentSettings);

        LoadSitesCommand.Execute(null);
        LoadSnippetsCommand.Execute(null); // Load snippets on start
    }

    private void OnSettingsChanged(AppSettings newSettings)
    {
        ApplySettings(newSettings);
    }

    private void ApplySettings(AppSettings s)
    {
        CurrentTheme = s.EditorTheme;
        EditorFontFamily = new FontFamily(s.EditorFontFamily);
        EditorFontSize = s.EditorFontSize;
    }

    #region snippets

    [RelayCommand]
    private void EditSnippet(NginxSnippet snippet)
    {
        if (snippet == null) return;
        EditSnippetRequested?.Invoke(snippet);
    }

    [RelayCommand]
    private async Task LoadSnippets()
    {
        var list = await _snippetService.LoadSnippetsAsync();
        Snippets.Clear();
        foreach (var s in list) Snippets.Add(s);
    }

    [RelayCommand]
    private void InsertSnippet(NginxSnippet snippet)
    {
        if (snippet == null) return;
        InsertTextRequested?.Invoke(snippet.Content);
        StatusMessage = $"Inserted snippet: {snippet.Name}";
    }

    [RelayCommand]
    private async Task AddSnippet()
    {
        var newSnippet = new NginxSnippet { Name = "New Snippet", Content = "# Config here" };
        Snippets.Add(newSnippet);
        SelectedSnippet = newSnippet;
        await _snippetService.SaveSnippetsAsync(Snippets);
    }

    [RelayCommand]
    private async Task RemoveSnippet(NginxSnippet snippet)
    {
        if (snippet != null && Snippets.Contains(snippet))
        {
            Snippets.Remove(snippet);
            await _snippetService.SaveSnippetsAsync(Snippets);
        }
    }

    // Called by the View after the Edit Window closes successfully
    public async Task SaveSnippetsExternal()
    {
        await _snippetService.SaveSnippetsAsync(Snippets);
        StatusMessage = "Snippets saved.";
    }

    #endregion

    #region sites

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

    #endregion
}