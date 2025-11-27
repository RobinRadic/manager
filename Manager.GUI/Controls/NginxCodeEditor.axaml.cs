using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using Manager.GUI.Services.Nginx;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;

namespace Manager.GUI.Controls;

public partial class NginxCodeEditor : UserControl
{
    private TextMate.Installation? _textMateInstallation;
    private NginxRegistryOptions? _registryOptions;

    #region Dependency Properties

    // 1. Text Property (TwoWay Binding)
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<NginxCodeEditor, string>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // 2. Save Command
    public static readonly StyledProperty<System.Windows.Input.ICommand> SaveCommandProperty =
        AvaloniaProperty.Register<NginxCodeEditor, System.Windows.Input.ICommand>(nameof(SaveCommand));

    public System.Windows.Input.ICommand SaveCommand
    {
        get => GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    // 3. SelectedTheme (TwoWay Binding) - To sync with Settings
    public static readonly StyledProperty<string> SelectedThemeProperty =
        AvaloniaProperty.Register<NginxCodeEditor, string>(nameof(SelectedTheme), "dark_vs", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string SelectedTheme
    {
        get => GetValue(SelectedThemeProperty);
        set => SetValue(SelectedThemeProperty, value);
    }

    #endregion

    public NginxCodeEditor()
    {
        InitializeComponent();
        InitializeTextMate();
    }

    private void InitializeTextMate()
    {
        var editor = this.FindControl<TextEditor>("Editor");
        if (editor == null) return;

        // Visual Options
        editor.Options.ShowSpaces = false;
        editor.Options.HighlightCurrentLine = true;

        // Sync Text <-> Editor
        editor.TextChanged += (s, e) => 
        { 
            if (Text != editor.Text) SetCurrentValue(TextProperty, editor.Text); 
        };

        // Initialize Registry & TextMate
        _registryOptions = new NginxRegistryOptions();
        _textMateInstallation = editor.InstallTextMate(_registryOptions);
        _textMateInstallation.SetGrammar("source.nginx");

        // Populate Theme List
        var themeList = this.FindControl<ListBox>("ThemeList");
        if (themeList != null)
        {
            themeList.ItemsSource = _registryOptions.GetAvailableThemes();
        }
        
        // Initial Theme Application
        ApplyTheme(SelectedTheme);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            var editor = this.FindControl<TextEditor>("Editor");
            if (editor != null && change.NewValue is string newText && editor.Text != newText)
            {
                editor.Text = newText;
            }
        }
        else if (change.Property == SelectedThemeProperty)
        {
            if (change.NewValue is string themeName)
            {
                ApplyTheme(themeName);
            }
        }
    }

    private void ThemeList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // When user picks from the ListBox, update the Property
        // The Property Changed handler above will then trigger ApplyTheme
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is string themeName)
        {
            SetCurrentValue(SelectedThemeProperty, themeName);
        }
    }

    private void ApplyTheme(string themeName)
    {
        if (_registryOptions == null || _textMateInstallation == null || string.IsNullOrEmpty(themeName)) return;

        var theme = _registryOptions.GetThemeByName(themeName);
        if (theme != null)
        {
            _textMateInstallation.SetTheme(theme);
            ApplyGuiColors(theme);
        }
    }

    private void ApplyGuiColors(IRawTheme theme)
    {
        var editor = this.FindControl<TextEditor>("Editor");
        if (editor == null) return;
        
        foreach (var setting in theme.GetGuiColors())
        {
            switch (setting.Key)
            {
                case "editor.background":
                    editor.Background = ParseBrush(setting.Value.ToString());
                    break;
                case "editor.foreground":
                    editor.Foreground = ParseBrush(setting.Value.ToString());
                    break;
                case "editor.selectionBackground":
                    editor.TextArea.SelectionBrush = ParseBrush(setting.Value.ToString());
                    break;
                case "editorCursor.foreground":
                    editor.TextArea.Caret.CaretBrush = ParseBrush(setting.Value.ToString());
                    break;
                default:
                    break;
            }
        }
    }

    private IBrush? ParseBrush(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return null;
        try
        {
            // Avalonia Color.Parse can handle #RGB, #RRGGBB, #AARRGGBB
            var color = Color.Parse(hex);
            return new SolidColorBrush(color);
        }
        catch
        {
            return null;
        }
    }


    // Public method for Snippet Manager
    public void InsertTextAtCursor(string text)
    {
        var editor = this.FindControl<TextEditor>("Editor");
        if (editor != null && !string.IsNullOrEmpty(text))
        {
            editor.TextArea.Document.Insert(editor.TextArea.Caret.Offset, text);
            editor.Focus();
        }
    }
}