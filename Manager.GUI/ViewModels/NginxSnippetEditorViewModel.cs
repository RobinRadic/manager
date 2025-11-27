using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.GUI.Services.Nginx;

namespace Manager.GUI.ViewModels;

public partial class NginxSnippetEditorViewModel : ViewModelBase
{
    private readonly NginxSnippet _originalSnippet;

    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
    [ObservableProperty] private string _content;
    
    // Editor settings passed from the main view to maintain consistency
    public string EditorTheme { get; }
    public FontFamily EditorFontFamily { get; }
    public double EditorFontSize { get; }

    // Event to request the view to close
    public event Action<bool>? CloseRequested;

    public NginxSnippetEditorViewModel(NginxSnippet snippet, string theme, FontFamily font, double fontSize)
    {
        _originalSnippet = snippet;
        _name = snippet.Name;
        _description = snippet.Description;
        _content = snippet.Content;
        
        EditorTheme = theme;
        EditorFontFamily = font;
        EditorFontSize = fontSize;
    }

    [RelayCommand]
    private void Save()
    {
        // specific validation could go here
        
        // Update the original object reference
        _originalSnippet.Name = Name;
        _originalSnippet.Description = Description;
        _originalSnippet.Content = Content;
        
        // Signal success
        CloseRequested?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(false);
    }
}