using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Manager.GUI.Controls;
using Manager.GUI.Services.Nginx;
using Manager.GUI.ViewModels;

namespace Manager.GUI.Views;

public partial class NginxView : UserControl
{
    private NginxViewModel? _vm;

    public NginxView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is NginxViewModel vm)
        {
            _vm = vm;

            // Subscribe to Insert Request from Snippet Manager
            vm.InsertTextRequested -= OnInsertTextRequested;
            vm.InsertTextRequested += OnInsertTextRequested;
            
            // Subscribe to Edit Request
            vm.EditSnippetRequested -= OnEditSnippetRequested;
            vm.EditSnippetRequested += OnEditSnippetRequested;
        }
    }

    private void OnInsertTextRequested(string text)
    {
        // Find our custom control
        var control = this.FindControl<NginxCodeEditor>("MainEditor");
        control?.InsertTextAtCursor(text);
    }

    private void OnEditSnippetRequested(NginxSnippet snippet)
    {
        if (_vm == null) return;

        // Create VM for editor, passing current theme settings
        var editorVm = new NginxSnippetEditorViewModel(
            snippet, 
            _vm.CurrentTheme, 
            _vm.EditorFontFamily, 
            _vm.EditorFontSize);

        var window = new NginxSnippetEditorWindow
        {
            DataContext = editorVm
        };

        // Find parent window to use as owner
        var topLevel = TopLevel.GetTopLevel(this) as Window;
        
        // Use Dispatcher to ensure UI thread interaction and proper async flow
        Dispatcher.UIThread.InvokeAsync(async () => 
        {
            bool result = false;
            if (topLevel != null)
            {
                result = await window.ShowDialog<bool>(topLevel);
            }
            else
            {
                // Fallback if not attached to a window yet (rare)
                window.Show(); 
            }

            if (result)
            {
                // If saved, persist the changes to disk via the main VM
                await _vm.SaveSnippetsExternal();
            }
        });
    }
    
    private void ChatBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _vm != null)
        {
            if (_vm.SubmitChatCommand.CanExecute(null))
            {
                _vm.SubmitChatCommand.Execute(null);
            }
        }
    }
}