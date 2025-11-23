using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Manager.Core.Features.Hosts;
using Manager.GUI.ViewModels;

namespace Manager.GUI.Views;

public partial class HostsView : UserControl
{
    public HostsView()
    {
        InitializeComponent();
        
        // Defer scrolling and selection until after the layout has been updated.
        Dispatcher.UIThread.Post(() =>
        {
            var dataGrid = this.FindControl<DataGrid>("HostEntriesGrid");
            if (dataGrid == null) return;
            var bar = dataGrid.GetTemplateChildren().OfType<ScrollBar>().Where(x => x.Name == "PART_VerticalScrollbar").First();
            bar.AllowAutoHide = false;
        }, DispatcherPriority.Loaded);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is HostsViewModel vm)
        {
            // Subscribe to the event
            vm.PreviewRequested -= ShowPreviewWindow;
            vm.PreviewRequested += ShowPreviewWindow;
            vm.EntryAdded -= OnEntryAdded;
            vm.EntryAdded += OnEntryAdded;
        }
    }

    private void OnEntryAdded(HostEntry entry)
    {
        var dataGrid = this.FindControl<DataGrid>("HostEntriesGrid");
        if (dataGrid != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                    dataGrid.ScrollIntoView(entry, null);
            }, DispatcherPriority.Loaded);
        }
    }

    private void ShowPreviewWindow(string content)
    {
        // Create the window
        var previewVm = new HostsPreviewViewModel(content);
        var window = new HostsPreviewWindow
        {
            DataContext = previewVm
        };

        // Find the parent window to center the popup
        var topLevel = TopLevel.GetTopLevel(this) as Window;
        if (topLevel != null)
        {
            window.ShowDialog(topLevel);
        }
        else
        {
            window.Show();
        }
    }
}