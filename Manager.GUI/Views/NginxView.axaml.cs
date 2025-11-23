using System;
using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaEdit;
using Manager.GUI.ViewModels;

namespace Manager.GUI.Views;

public partial class NginxView : UserControl
{
    private NginxViewModel? _vm;

    public NginxView()
    {
        InitializeComponent();
        
        // Setup Editor Options
        var editor = this.FindControl<TextEditor>("ConfigEditor");
        if (editor != null)
        {
            editor.Options.ShowSpaces = false;
            editor.Options.HighlightCurrentLine = true;
            
            // When user types, update the ViewModel
            editor.TextChanged += (s, e) =>
            {
                if (_vm != null) _vm.CurrentConfigText = editor.Text;
            };
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is NginxViewModel vm)
        {
            _vm = vm;
            
            // Listen for changes FROM the ViewModel (e.g. when a new site is selected)
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
    }
}