using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion; // Required for CompletionWindow
using AvaloniaEdit.Document; // Required for Document handling
using AvaloniaEdit.Editing; // Required for TextArea/Caret
using AvaloniaEdit.TextMate;
using Manager.GUI.Services.Nginx;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Manager.GUI.Controls;

public partial class NginxCodeEditor : UserControl
{
    private TextEditor? _editor;
    private TextMate.Installation? _textMateInstallation;
    private NginxRegistryOptions? _registryOptions;
    private CompletionWindow? _completionWindow; // The missing Completion Window

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<NginxCodeEditor, string>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string> SelectedThemeProperty =
        AvaloniaProperty.Register<NginxCodeEditor, string>(nameof(SelectedTheme), "dark_vs", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string SelectedTheme
    {
        get => GetValue(SelectedThemeProperty);
        set => SetValue(SelectedThemeProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> ExplainCommandProperty =
        AvaloniaProperty.Register<NginxCodeEditor, ICommand>(nameof(ExplainCommand));

    public ICommand ExplainCommand
    {
        get => GetValue(ExplainCommandProperty);
        set => SetValue(ExplainCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> FixCommandProperty =
        AvaloniaProperty.Register<NginxCodeEditor, ICommand>(nameof(FixCommand));

    public ICommand FixCommand
    {
        get => GetValue(FixCommandProperty);
        set => SetValue(FixCommandProperty, value);
    }

    public NginxCodeEditor()
    {
        InitializeComponent();
        _editor = this.FindControl<TextEditor>("Editor");
        InitializeTextMate();
    }
    
    [RelayCommand]
    private void ExecuteExplain()
    {
        var text = _editor?.SelectedText;
        if (ExplainCommand?.CanExecute(text) == true)
        {
            ExplainCommand.Execute(text);
        }
    }

    [RelayCommand]
    private void ExecuteFix()
    {
        var text = _editor?.SelectedText;
        if (FixCommand?.CanExecute(text) == true)
        {
            FixCommand.Execute(text);
        }
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

        // Wire up Completion Events (Restored functionality)
        editor.TextArea.TextEntered += TextArea_TextEntered;
        editor.TextArea.TextEntering += TextArea_TextEntering;
        editor.TextArea.KeyDown += TextArea_KeyDown;

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


    protected TextEditor TextEditor => this.FindControl<TextEditor>("Editor")!;

    #region Completion Logic (Restored)
    private void TextArea_TextEntering(object? sender, TextInputEventArgs e)
    {
        if (e.Text.Length > 0 && _completionWindow != null)
        {
            // If user types a space or special char, close the window
            // Allow '$' and '_' as they are valid in Nginx directives/variables
            if (!char.IsLetterOrDigit(e.Text[0]) && e.Text[0] != '$' && e.Text[0] != '_')
            {
                _completionWindow.CompletionList.RequestInsertion(e);
            }
        }
    }

    private void TextArea_TextEntered(object? sender, TextInputEventArgs e)
    {
        if (e.Text.Length == 0) return;

        // 1. Always trigger if the user explicitly types '$' (Variable start)
        // This applies regardless of position (arguments or directives)
        if (e.Text[0] == '$')
        {
            ShowCompletionWindow();
            return;
        }

        var textArea = (TextArea)sender;

        // 2. For normal letters, ONLY trigger if we are typing a Directive (Start of line)
        // If we are past the first word (Argument context), suppress completion to avoid noise.
        if (char.IsLetter(e.Text[0]) && IsTypingDirective(textArea))
        {
            ShowCompletionWindow();
        }
    }

    /// <summary>
    /// Checks if the caret is currently editing the first word on the line (Directive context).
    /// </summary>
    private bool IsTypingDirective(TextArea textArea)
    {
        var line = textArea.Document.GetLineByOffset(textArea.Caret.Offset);
        // Get text from start of line up to caret
        var textBeforeCaret = textArea.Document.GetText(line.Offset, textArea.Caret.Offset - line.Offset);

        // Remove leading whitespace (indentation)
        var trimmed = textBeforeCaret.TrimStart();

        // If there is any whitespace left, it means we have passed the first word 
        // and are likely typing arguments.
        // IndexOfAny returns -1 if no space/tab is found.
        return trimmed.IndexOfAny(new[] { ' ', '\t' }) == -1;
    }

    private void TextArea_KeyDown(object? sender, KeyEventArgs e)
    {
        // Check for Ctrl + Space to trigger manually
        if (e.Key == Key.Space && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            ShowCompletionWindow();
            e.Handled = true;
        }
    }

    private void ShowCompletionWindow()
    {
        var editor = this.FindControl<TextEditor>("Editor");
        if (editor == null || _completionWindow != null) return;

        // Create the window
        _completionWindow = new CompletionWindow(editor.TextArea);

        // Get the current word being typed to filter the list
        string currentWord = GetCurrentWord(editor.TextArea);
        
        // IMPORTANT: Adjust the StartOffset to include the characters already typed.
        // This ensures "add_he" is replaced by "add_header" instead of becoming "add_headd_header".
        _completionWindow.StartOffset -= currentWord.Length;

        var data = _completionWindow.CompletionList.CompletionData;

        // Add Keywords matching user input
        foreach (var keyword in NginxKeywords.All)
        {
            // Using StartsWith for filtering
            if (keyword.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase))
            {
                data.Add(new NginxCompletionData(keyword));
            }
        }

        if (data.Count > 0)
        {
            _completionWindow.Show();
            _completionWindow.Closed += (o, args) => _completionWindow = null;
        }
        else
        {
            _completionWindow = null;
        }
    }

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

        start++; // Adjust to point to start of word

        return textArea.Document.GetText(start, offset - start);
    }

    #endregion

    #region Theming & Helper Logic

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

    public void InsertTextAtCursor(string text)
    {
        var editor = this.FindControl<TextEditor>("Editor");
        if (editor != null && !string.IsNullOrEmpty(text))
        {
            editor.TextArea.Document.Insert(editor.TextArea.Caret.Offset, text);
            editor.Focus();
        }
    }

    #endregion
}