using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.GUI.Services.Nginx;
using Manager.GUI.Services.Settings;

namespace Manager.GUI.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly NginxRegistryOptions _nginxOptions;

    // -- App Theme --
    [ObservableProperty] private string _selectedAppTheme;
    public ObservableCollection<string> AppThemes { get; } = new() { "Dark", "Light" };

    // -- Editor Theme --
    [ObservableProperty] private string _selectedEditorTheme;
    public ObservableCollection<string> EditorThemes { get; } = new();

    // -- Editor Fonts --
    [ObservableProperty] private string _selectedFont;
    public ObservableCollection<string> InstalledFonts { get; } = new();

    // -- Editor Font Size --
    [ObservableProperty] private double _fontSize;

    public SettingsViewModel(SettingsService settingsService, NginxRegistryOptions nginxOptions)
    {
        _settingsService = settingsService;
        _nginxOptions = nginxOptions;

        // Load Data
        LoadOptions();
        LoadCurrentSettings();
    }

    private void LoadOptions()
    {
        // 1. Get Editor Themes
        var themes = _nginxOptions.GetAvailableThemes();
        foreach (var t in themes) EditorThemes.Add(t);

        // 2. Get System Fonts
        var fonts = FontManager.Current.SystemFonts;
        foreach (var f in fonts.OrderBy(x => x.Name))
        {
            InstalledFonts.Add(f.Name);
        }
    }

    private void LoadCurrentSettings()
    {
        var s = _settingsService.CurrentSettings;
        SelectedAppTheme = s.AppTheme;
        SelectedEditorTheme = s.EditorTheme;
        SelectedFont = s.EditorFontFamily;
        FontSize = s.EditorFontSize;
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        var newSettings = new AppSettings
        {
            AppTheme = SelectedAppTheme,
            EditorTheme = SelectedEditorTheme,
            EditorFontFamily = SelectedFont,
            EditorFontSize = FontSize
        };

        await _settingsService.SaveSettingsAsync(newSettings);
        
        // Apply App Theme Immediately
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = SelectedAppTheme == "Dark" 
                ? ThemeVariant.Dark 
                : ThemeVariant.Light;
        }
    }
}