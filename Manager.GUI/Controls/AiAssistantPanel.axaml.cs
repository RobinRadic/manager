using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using LiveMarkdown.Avalonia;

namespace Manager.GUI.Controls;

public partial class AiAssistantPanel : UserControl
{
    public AiAssistantPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // --- Styled Properties (Bindable) ---

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<AiAssistantPanel, string>(nameof(Title), "AI Assistant");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<IEnumerable> MessagesProperty =
        AvaloniaProperty.Register<AiAssistantPanel, IEnumerable>(nameof(Messages));

    public IEnumerable Messages
    {
        get => GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    public static readonly StyledProperty<string> QueryProperty =
        AvaloniaProperty.Register<AiAssistantPanel, string>(nameof(Query), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Query
    {
        get => GetValue(QueryProperty);
        set => SetValue(QueryProperty, value);
    }

    public static readonly StyledProperty<bool> IsBusyProperty =
        AvaloniaProperty.Register<AiAssistantPanel, bool>(nameof(IsBusy));

    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly StyledProperty<ICommand> SubmitCommandProperty =
        AvaloniaProperty.Register<AiAssistantPanel, ICommand>(nameof(SubmitCommand));

    public ICommand SubmitCommand
    {
        get => GetValue(SubmitCommandProperty);
        set => SetValue(SubmitCommandProperty, value);
    }

    // --- Event Handlers ---

    private void ChatBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.None)
        {
            if (SubmitCommand?.CanExecute(null) == true)
            {
                SubmitCommand.Execute(null);
            }
        }
    }

    // --- Attached Property for LiveMarkdown Integration ---
    // We keep this here so the DataTemplate inside the XAML can use it comfortably.

    public static readonly AttachedProperty<string> MarkdownContentProperty =
        AvaloniaProperty.RegisterAttached<AiAssistantPanel, MarkdownRenderer, string>(
            "MarkdownContent");

    public static string GetMarkdownContent(Control element)
    {
        return element.GetValue(MarkdownContentProperty);
    }

    public static void SetMarkdownContent(Control element, string value)
    {
        element.SetValue(MarkdownContentProperty, value);
    }

    static AiAssistantPanel()
    {
        MarkdownContentProperty.Changed.AddClassHandler<MarkdownRenderer>(OnMarkdownContentChanged);
    }

    private static void OnMarkdownContentChanged(MarkdownRenderer renderer, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is string newText)
        {
            if (renderer.MarkdownBuilder == null)
            {
                renderer.MarkdownBuilder = new ObservableStringBuilder();
            }

            renderer.MarkdownBuilder.Clear();
            renderer.MarkdownBuilder.Append(newText);
        }
    }
}