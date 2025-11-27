using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using Manager.GUI.Services.Nginx;
using Manager.GUI.ViewModels;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;

namespace Manager.GUI.Views;

public partial class NginxView : UserControl
{
    private NginxViewModel? _vm;
    private NginxRegistryOptions? _registryOptions;
    private TextMate.Installation? _textMateInstallation;
    private CompletionWindow? _completionWindow;

    public NginxView()
    {
        InitializeComponent();
        InitializeEditor();
    }

    private void InitializeEditor()
    {
        // Setup Editor Options
        var editor = this.FindControl<TextEditor>("ConfigEditor");
        if (editor != null)
        {
            _registryOptions = new NginxRegistryOptions();
            _textMateInstallation = editor.InstallTextMate(_registryOptions);
            _textMateInstallation.SetGrammar("source.nginx");

            editor.Options.ShowSpaces = false;
            editor.Options.HighlightCurrentLine = true;

            // When user types, update the ViewModel
            editor.TextChanged += (s, e) =>
            {
                if (_vm != null) _vm.CurrentConfigText = editor.Text;
            };
            editor.TextArea.TextEntered += TextArea_TextEntered;
            editor.TextArea.TextEntering += TextArea_TextEntering;
            editor.TextArea.KeyDown += TextArea_KeyDown;
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is NginxViewModel vm)
        {
            _vm = vm;

            // Populate Available Themes from our RegistryOptions helper
            if (_registryOptions != null)
            {
                var themes = _registryOptions.GetAvailableThemes();
                vm.AvailableThemes.Clear();
                foreach (var t in themes)
                {
                    vm.AvailableThemes.Add(t);
                }

                // Default theme if none selected
                if (string.IsNullOrEmpty(vm.CurrentTheme))
                {
                    vm.CurrentTheme = "dark_vs";
                }
            }

            // Apply the current theme immediately
            ApplyTheme(vm.CurrentTheme);

            // Listen for changes FROM the ViewModel (e.g. when a new site is selected or theme changes)
            vm.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NginxViewModel.CurrentConfigText))
        {
            var editor = this.FindControl<TextEditor>("ConfigEditor");

            // Only update if the text is actually different to avoid cursor jumping
            if (editor != null && editor.Text != _vm?.CurrentConfigText)
            {
                editor.Text = _vm?.CurrentConfigText ?? "";
            }
        }
        else if (e.PropertyName == nameof(NginxViewModel.CurrentTheme))
        {
            if (_vm != null)
            {
                ApplyTheme(_vm.CurrentTheme);
            }
        }
    }

    private void ApplyTheme(string themeName)
    {
        if (_registryOptions != null && _textMateInstallation != null && !string.IsNullOrEmpty(themeName))
        {
            // Load the specific theme by name
            var theme = _registryOptions.GetThemeByName(themeName);

            if (theme != null)
            {
                _textMateInstallation.SetTheme(theme);
                ApplyGuiColors(theme);
            }
        }
    }

    private void ApplyGuiColors(IRawTheme theme)
    {
        var editor = this.FindControl<TextEditor>("ConfigEditor");
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
        // 5. Line Highlight (Optional)
        // AvaloniaEdit handles line highlighting internally, but setting the background explicitly 
        // for the line highlighter requires more deep access (ITextViewLineTransformer). 
        // For now, the TextMate installation usually handles syntax coloring, 
        // but the main editor background must be set manually as done above.
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

    // 1. Handle Closing the window if user types non-completion characters
    private void TextArea_TextEntering(object? sender, TextInputEventArgs e)
    {
        if (e.Text.Length > 0 && _completionWindow != null)
        {
            // If user types a space or special char, close the window
            if (!char.IsLetterOrDigit(e.Text[0]) && e.Text[0] != '$' && e.Text[0] != '_')
            {
                _completionWindow.CompletionList.RequestInsertion(e);
            }
        }
    }

    // 2. Handle Opening the window
    private void TextArea_TextEntered(object? sender, TextInputEventArgs e)
    {
        if (e.Text.Length == 0) return;
        // Trigger completion only on letters, '$' or '_'
        // You can adjust this logic (e.g. require at least 2 chars)
        if (char.IsLetter(e.Text[0]) || e.Text[0] == '$' || e.Text[0] == '_')
        {
            ShowCompletionWindow();
        }
    }

    private void TextArea_KeyDown(object? sender, KeyEventArgs e)
    {
        // Check for Ctrl + Space
        if (e.Key == Key.Space && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            // Trigger the completion window manually
            ShowCompletionWindow();

            // IMPORTANT: Mark as handled so a space isn't actually inserted
            e.Handled = true;
        }
    }

    private void ShowCompletionWindow()
    {
        // Clean up existing window
        if (_completionWindow != null) return;

        // Create the window
        _completionWindow = new CompletionWindow(ConfigEditor.TextArea);

        // Get the current word being typed to filter the list
        string currentWord = GetCurrentWord(ConfigEditor.TextArea);

        var data = _completionWindow.CompletionList.CompletionData;

        // Add Keywords that match what is being typed
        foreach (var keyword in NginxKeywords.All)
        {
            // "StartsWith" filter. Change to "Contains" if you prefer fuzzy search.
            if (keyword.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase))
            {
                data.Add(new NginxCompletionData(keyword));
            }
        }

        // Only show if we have matches
        if (data.Count > 0)
        {
            _completionWindow.Show();
            _completionWindow.Closed += (o, args) => _completionWindow = null;
        }
        else
        {
            _completionWindow = null; // Clean up immediately if empty
        }
    }

    // Helper: Get the word directly before the caret
    private string GetCurrentWord(TextArea textArea)
    {
        int offset = textArea.Caret.Offset;
        if (offset == 0) return "";

        int start = offset - 1;
        while (start >= 0)
        {
            char c = textArea.Document.GetCharAt(start);
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '$')
            {
                break;
            }

            start--;
        }

        // Adjust start to point to first char of word
        start++;

        return textArea.Document.GetText(start, offset - start);
    }
}