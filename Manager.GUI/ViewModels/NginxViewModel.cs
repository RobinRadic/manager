using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.Core.Features.Nginx;
using Manager.GUI.Services.AI;
using Manager.GUI.Services.Nginx;
using Manager.GUI.Services.Settings;

namespace Manager.GUI.ViewModels;

public partial class NginxViewModel : ViewModelBase
{
    private readonly NginxService _nginxService;
    private readonly SettingsService _settingsService;
    private readonly NginxSnippetService _snippetService; // New Dependency
    private readonly AiService _aiService;
    
    // Events
    public event Action<string>? InsertTextRequested; // View subscribes to this
    public event Action<NginxSnippet>? EditSnippetRequested; // View subscribes to this

    // Collections
    [ObservableProperty] private ObservableCollection<NginxSite> _sites = new();
    [ObservableProperty] private ObservableCollection<NginxSnippet> _snippets = new(); // New Collection
    [ObservableProperty] private ObservableCollection<NginxConfigFile> _configFiles = new(); // NEW
    
    // AI Panel Props
    [ObservableProperty] private bool _isAiPanelOpen = false;
    [ObservableProperty] private string _userQuery = "";
    [ObservableProperty] private ObservableCollection<ChatMessage> _chatMessages = new();
    [ObservableProperty] private bool _isAiBusy;
    
    // Resizing Props
    [ObservableProperty] private GridLength _aiColumnWidth = new GridLength(0); // Default closed
    [ObservableProperty] private double _aiPanelMinWidth = 0; // Default 0 to allow collapse
    private double _lastAiPanelWidth = 300; // Default width when opened
    
    // Properties
    [ObservableProperty] private NginxSite? _selectedSite;
    [ObservableProperty] private NginxSnippet? _selectedSnippet; // Selection for editing
    [ObservableProperty] private NginxConfigFile? _selectedConfigFile;
    
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
        NginxSnippetService snippetService,
        AiService aiService) // Inject
    {
        _nginxService = nginxService;
        _settingsService = settingsService;
        _snippetService = snippetService;
        _aiService = aiService;

        _settingsService.SettingsChanged += OnSettingsChanged;
        ApplySettings(_settingsService.CurrentSettings);

        LoadSitesCommand.Execute(null);
        LoadSnippetsCommand.Execute(null); // Load snippets on start
        LoadConfigFilesCommand.Execute(null);
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

    #region AI
    partial void OnIsAiPanelOpenChanged(bool value)
    {
        if (value)
        {
            // Opening: Restore width and set min width
            AiPanelMinWidth = 200;
            AiColumnWidth = new GridLength(_lastAiPanelWidth > 200 ? _lastAiPanelWidth : 300);
        }
        else
        {
            // Closing: Save current width (if valid) and collapse
            if (AiColumnWidth.Value >= 200) _lastAiPanelWidth = AiColumnWidth.Value;
            
            AiPanelMinWidth = 0;
            AiColumnWidth = new GridLength(0);
        }
    }
    
    [RelayCommand]
    private void ToggleAiPanel()
    {
        IsAiPanelOpen = !IsAiPanelOpen;
    }

    [RelayCommand]
    private async Task SubmitChat()
    {
        if (string.IsNullOrWhiteSpace(UserQuery)) return;

        IsAiBusy = true;
        ChatMessages.Add(new ChatMessage("User", UserQuery));
        var query = UserQuery;
        UserQuery = "";

        // Add loading indicator
        var loadingMsg = new ChatMessage("Gemini", "Thinking...", true);
        ChatMessages.Add(loadingMsg);

        var response = await _aiService.ChatAsync(query, CurrentConfigText);
        
        // Remove loading and add response
        ChatMessages.Remove(loadingMsg);
        ChatMessages.Add(new ChatMessage("Gemini", response));
        
        IsAiBusy = false;
    }

    [RelayCommand]
    private async Task ExplainSelection(string selection)
    {
        if (string.IsNullOrWhiteSpace(selection)) return;
        
        IsAiPanelOpen = true; 
        IsAiBusy = true;
        ChatMessages.Add(new ChatMessage("User", $"Explain this:\n{selection}"));
        
        var loadingMsg = new ChatMessage("Gemini", "Analyzing...", true);
        ChatMessages.Add(loadingMsg);
        
        var response = await _aiService.ExplainCodeAsync(selection);
        
        ChatMessages.Remove(loadingMsg);
        ChatMessages.Add(new ChatMessage("Gemini", response));
        
        IsAiBusy = false;
    }

    [RelayCommand]
    private async Task FixSelection(string selection)
    {
        if (string.IsNullOrWhiteSpace(selection)) return;

        IsAiPanelOpen = true; 
        IsAiBusy = true;
        ChatMessages.Add(new ChatMessage("User", $"Fix/Optimize this:\n{selection}"));

        var loadingMsg = new ChatMessage("Gemini", "Fixing...", true);
        ChatMessages.Add(loadingMsg);

        var response = await _aiService.FixCodeAsync(selection);
        
        ChatMessages.Remove(loadingMsg);
        ChatMessages.Add(new ChatMessage("Gemini", response));
        
        IsAiBusy = false;
    }

    #endregion

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

    #region Config Files (NEW REGION)

    [RelayCommand]
    private async Task LoadConfigFiles()
    {
        var configs = await _nginxService.LoadConfigFilesAsync();
        ConfigFiles.Clear();
        foreach (var c in configs) ConfigFiles.Add(c);
    }

    // When a Config File is selected
    partial void OnSelectedConfigFileChanged(NginxConfigFile? value)
    {
        if (value != null)
        {
            // Deselect others to avoid confusion
            SelectedSite = null;
            SelectedSnippet = null;

            LoadConfigFromFile(value);
        }
    }

    private async void LoadConfigFromFile(NginxConfigFile file)
    {
        try
        {
            StatusMessage = $"Reading {file.Name}...";
            // Simple read, assuming the service handles permissions or we are root
            CurrentConfigText = await Task.Run(() => File.ReadAllText(file.FilePath));
            StatusMessage = $"Loaded {file.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
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
            // Deselect others
            SelectedConfigFile = null; 
            SelectedSnippet = null;

            LoadConfigForSite(value);
        }
        else if (SelectedConfigFile == null && SelectedSnippet == null)
        {
            // Only clear text if nothing else is selected
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
        // 1. Save Site
        if (SelectedSite != null)
        {
            try
            {
                StatusMessage = "Saving Site...";
                await _nginxService.SaveConfigAsync(SelectedSite.Name, CurrentConfigText);
                StatusMessage = "Site saved & Nginx reloaded.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"SAVE FAILED: {ex.Message}";
            }
            return;
        }

        // 2. Save Config File (NEW)
        if (SelectedConfigFile != null)
        {
            try
            {
                StatusMessage = $"Saving {SelectedConfigFile.Name}...";
                await _nginxService.SaveFileAsync(SelectedConfigFile.FilePath, CurrentConfigText);
                StatusMessage = "Config saved.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"SAVE FAILED: {ex.Message}";
            }
            return;
        }
        
        StatusMessage = "Nothing selected to save.";
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