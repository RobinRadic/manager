using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.Input;
using Manager.GUI.Services.Php;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;

namespace Manager.GUI.Controls;

public partial class PhpCodeEditor : UserControl
{
    private TextEditor? _editor;
    private TextMate.Installation? _textMateInstallation;
    private PhpRegistryOptions? _registryOptions;
    private CompletionWindow? _completionWindow;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<PhpCodeEditor, string>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string> SelectedThemeProperty =
        AvaloniaProperty.Register<PhpCodeEditor, string>(nameof(SelectedTheme), "dark_vs", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string SelectedTheme
    {
        get => GetValue(SelectedThemeProperty);
        set => SetValue(SelectedThemeProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> ExplainCommandProperty =
        AvaloniaProperty.Register<PhpCodeEditor, ICommand>(nameof(ExplainCommand));

    public ICommand ExplainCommand
    {
        get => GetValue(ExplainCommandProperty);
        set => SetValue(ExplainCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> FixCommandProperty =
        AvaloniaProperty.Register<PhpCodeEditor, ICommand>(nameof(FixCommand));

    public ICommand FixCommand
    {
        get => GetValue(FixCommandProperty);
        set => SetValue(FixCommandProperty, value);
    }

    public PhpCodeEditor()
    {
        InitializeComponent();
        _editor = this.FindControl<TextEditor>("Editor");
        InitializeTextMate();
    }
    
    [RelayCommand]
    private void ExecuteExplain()
    {
        var text = _editor?.SelectedText;
        if (ExplainCommand?.CanExecute(text) == true) ExplainCommand.Execute(text);
    }

    [RelayCommand]
    private void ExecuteFix()
    {
        var text = _editor?.SelectedText;
        if (FixCommand?.CanExecute(text) == true) FixCommand.Execute(text);
    }
    
    private void InitializeTextMate()
    {
        var editor = this.FindControl<TextEditor>("Editor");
        if (editor == null) return;

        editor.Options.ShowSpaces = false;
        editor.Options.HighlightCurrentLine = true;

        editor.TextChanged += (s, e) =>
        {
            if (Text != editor.Text) SetCurrentValue(TextProperty, editor.Text);
        };

        editor.TextArea.TextEntered += TextArea_TextEntered;
        editor.TextArea.TextEntering += TextArea_TextEntering;
        editor.TextArea.KeyDown += TextArea_KeyDown;

        _registryOptions = new PhpRegistryOptions();
        _textMateInstallation = editor.InstallTextMate(_registryOptions);
        // Changed to source.ini to match the new grammar
        _textMateInstallation.SetGrammar("source.ini");

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
            if (change.NewValue is string themeName) ApplyTheme(themeName);
        }
    }

    private void TextArea_TextEntering(object? sender, TextInputEventArgs e)
    {
        if (e.Text.Length > 0 && _completionWindow != null)
        {
            if (!char.IsLetterOrDigit(e.Text[0]) && e.Text[0] != '_' && e.Text[0] != '.')
            {
                _completionWindow.CompletionList.RequestInsertion(e);
            }
        }
    }

    private void TextArea_TextEntered(object? sender, TextInputEventArgs e)
    {
        if (e.Text.Length == 0) return;
        
        // Trigger on letters or dots (common in php.ini keys like opcache.enable)
        if (char.IsLetter(e.Text[0]) || e.Text[0] == '.')
        {
            ShowCompletionWindow();
        }
    }

    private void TextArea_KeyDown(object? sender, KeyEventArgs e)
    {
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

        _completionWindow = new CompletionWindow(editor.TextArea);
        string currentWord = GetCurrentWord(editor.TextArea);
        
        // Filter list based on current word
        var data = _completionWindow.CompletionList.CompletionData;

        foreach (var keyword in PhpKeywords.All)
        {
            if (keyword.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase))
            {
                data.Add(new PhpCompletionData(keyword));
            }
        }

        if (data.Count > 0)
        {
            _completionWindow.StartOffset -= currentWord.Length;
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
            // php.ini keys contain dots and underscores
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '.') break;
            start--;
        }
        start++;
        return textArea.Document.GetText(start, offset - start);
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
            if (setting.Key == "editor.background") editor.Background = ParseBrush(setting.Value.ToString());
            if (setting.Key == "editor.foreground") editor.Foreground = ParseBrush(setting.Value.ToString());
            if (setting.Key == "editor.selectionBackground") editor.TextArea.SelectionBrush = ParseBrush(setting.Value.ToString());
            if (setting.Key == "editorCursor.foreground") editor.TextArea.Caret.CaretBrush = ParseBrush(setting.Value.ToString());
        }
    }

    private IBrush? ParseBrush(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return null;
        try { return new SolidColorBrush(Color.Parse(hex)); } catch { return null; }
    }
}