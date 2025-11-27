using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Manager.GUI.ViewModels;

namespace Manager.GUI.Views;

public partial class NginxSnippetEditorWindow : Window
{
    public NginxSnippetEditorWindow()
    {
        InitializeComponent();
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is NginxSnippetEditorViewModel vm)
        {
            vm.CloseRequested += (result) =>
            {
                Close(result);
            };
        }
    }
}