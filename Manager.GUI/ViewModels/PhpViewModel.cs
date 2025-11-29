using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.Core.Features.Php;
using Manager.GUI.Services.AI;
using Manager.GUI.Services.Settings;

namespace Manager.GUI.ViewModels;

public partial class PhpViewModel : ViewModelBase
{
    private readonly PhpService _phpService;
    private readonly SettingsService _settingsService;
    private readonly AiService _aiService;

    // Collections
    [ObservableProperty] private ObservableCollection<string> _versions = new();
    [ObservableProperty] private ObservableCollection<PhpModule> _modules = new();
    [ObservableProperty] private ObservableCollection<ChatMessage> _chatMessages = new();

    // Selection
    [ObservableProperty] private string? _selectedVersion;
    [ObservableProperty] private PhpModule? _selectedModule;
    [ObservableProperty] private string _currentConfigText = "";
    [ObservableProperty] private string _statusMessage = "Ready";

    // AI Panel
    [ObservableProperty] private bool _isAiPanelOpen;
    [ObservableProperty] private bool _isAiBusy;
    [ObservableProperty] private string _userQuery = "";
    [ObservableProperty] private GridLength _aiColumnWidth = new GridLength(0);
    [ObservableProperty] private double _aiPanelMinWidth = 0;
    private double _lastAiPanelWidth = 300;

    // Editor Settings
    [ObservableProperty] private string _currentTheme = "dark_vs";
    [ObservableProperty] private FontFamily _editorFontFamily;
    [ObservableProperty] private double _editorFontSize;

    public PhpViewModel(PhpService phpService, SettingsService settingsService, AiService aiService)
    {
        _phpService = phpService;
        _settingsService = settingsService;
        _aiService = aiService;

        _settingsService.SettingsChanged += OnSettingsChanged;
        ApplySettings(_settingsService.CurrentSettings);

        LoadVersionsCommand.Execute(null);
    }

    private void OnSettingsChanged(AppSettings s) => ApplySettings(s);

    private void ApplySettings(AppSettings s)
    {
        CurrentTheme = s.EditorTheme;
        EditorFontFamily = new FontFamily(s.EditorFontFamily);
        EditorFontSize = s.EditorFontSize;
    }

    [RelayCommand]
    private void LoadVersions()
    {
        Versions.Clear();
        var v = _phpService.GetInstalledVersions();
        foreach (var ver in v) Versions.Add(ver);

        if (Versions.Count > 0) SelectedVersion = Versions.First();
    }

    partial void OnSelectedVersionChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            LoadModulesForVersion(value);
        }
    }

    private async void LoadModulesForVersion(string version)
    {
        try
        {
            StatusMessage = $"Loading modules for PHP {version}...";
            var mods = await _phpService.GetModulesAsync(version);
            Modules.Clear();
            foreach (var m in mods) Modules.Add(m);
            StatusMessage = $"Loaded {mods.Count} modules.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading modules: {ex.Message}";
        }
    }

    partial void OnSelectedModuleChanged(PhpModule? value)
    {
        if (value != null)
        {
            LoadConfig(value);
        }
    }

    private async void LoadConfig(PhpModule module)
    {
        try
        {
            StatusMessage = $"Reading {module.Name}...";
            CurrentConfigText = await _phpService.GetConfigContentAsync(module.FilePath);
            StatusMessage = $"Read {module.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error reading file: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveConfig()
    {
        if (SelectedModule == null) return;
        try
        {
            StatusMessage = "Saving...";
            await _phpService.SaveConfigAsync(SelectedModule.FilePath, CurrentConfigText);
            StatusMessage = "Saved successfully. (Restart PHP-FPM to apply)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save Failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ToggleModule(PhpModule module)
    {
        if (module == null) return;
        try
        {
            bool newState = !module.IsEnabled;
            StatusMessage = newState ? $"Enabling {module.Name}..." : $"Disabling {module.Name}...";
            
            await _phpService.ToggleModuleAsync(module, newState);
            
            // Refresh the specific module item in the list to update UI checkmark
            module.IsEnabled = newState;
            
            // We need to trigger the UI update. 
            // Since ObservableCollection tracks items, replacing the item works reliably.
            var idx = Modules.IndexOf(module);
            if (idx != -1) 
            {
                var newModule = new PhpModule 
                { 
                    Name = module.Name, 
                    FilePath = module.FilePath, 
                    Version = module.Version, 
                    IsEnabled = newState 
                };
                Modules[idx] = newModule;
                
                // If it was selected, re-select it
                if (SelectedModule == module) SelectedModule = newModule;
            }
            StatusMessage = newState ? $"Enabled {module.Name}" : $"Disabled {module.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Toggle Error: {ex.Message}";
        }
    }

    // AI Logic
    partial void OnIsAiPanelOpenChanged(bool value)
    {
        if (value) { AiPanelMinWidth = 200; AiColumnWidth = new GridLength(_lastAiPanelWidth > 200 ? _lastAiPanelWidth : 300); }
        else { if (AiColumnWidth.Value >= 200) _lastAiPanelWidth = AiColumnWidth.Value; AiPanelMinWidth = 0; AiColumnWidth = new GridLength(0); }
    }

    [RelayCommand]
    private void ToggleAiPanel() => IsAiPanelOpen = !IsAiPanelOpen;

    [RelayCommand]
    private async Task SubmitChat()
    {
        if (string.IsNullOrWhiteSpace(UserQuery)) return;
        IsAiBusy = true;
        ChatMessages.Add(new ChatMessage("User", UserQuery));
        var query = UserQuery; UserQuery = "";
        ChatMessages.Add(new ChatMessage("Gemini", "Thinking...", true));
        var response = await _aiService.ChatAsync(query, CurrentConfigText);
        ChatMessages.Remove(ChatMessages.Last());
        ChatMessages.Add(new ChatMessage("Gemini", response));
        IsAiBusy = false;
    }

    [RelayCommand]
    private async Task ExplainSelection(string selection)
    {
        if (string.IsNullOrWhiteSpace(selection)) return;
        IsAiPanelOpen = true; IsAiBusy = true;
        ChatMessages.Add(new ChatMessage("User", $"Explain this config:\n{selection}"));
        ChatMessages.Add(new ChatMessage("Gemini", "Analyzing INI...", true));
        var response = await _aiService.ExplainCodeAsync(selection);
        ChatMessages.Remove(ChatMessages.Last());
        ChatMessages.Add(new ChatMessage("Gemini", response));
        IsAiBusy = false;
    }

    [RelayCommand]
    private async Task FixSelection(string selection)
    {
        // For INI, "Fix" might mean correcting syntax or deprecations
        if (string.IsNullOrWhiteSpace(selection)) return;
        IsAiPanelOpen = true; IsAiBusy = true;
        ChatMessages.Add(new ChatMessage("User", $"Fix this configuration:\n{selection}"));
        ChatMessages.Add(new ChatMessage("Gemini", "Checking...", true));
        var response = await _aiService.FixCodeAsync(selection);
        ChatMessages.Remove(ChatMessages.Last());
        ChatMessages.Add(new ChatMessage("Gemini", response));
        IsAiBusy = false;
    }
}