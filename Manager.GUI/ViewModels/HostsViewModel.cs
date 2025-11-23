using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Manager.Core.Features.Hosts;

namespace Manager.GUI.ViewModels;

public partial class HostsViewModel : ViewModelBase
{
    private readonly HostsService _hostsService;

    // We use this event to tell the View to open a window
    public event Action<string>? PreviewRequested;
    
    // We use this event to tell the View to scroll to the bottom
    public event Action<HostEntry>? EntryAdded;

    [ObservableProperty]
    private ObservableCollection<HostEntry> _hostEntries = new();

    [ObservableProperty]
    private HostEntry? _selectedEntry;
    
    public IEnumerable<HostEntry> VisibleHostEntries => HostEntries.Where(e => !e.IsBlank && !e.IsPureComment);

    public HostsViewModel(HostsService hostsService)
    {
        _hostsService = hostsService;
        
        HostEntries.CollectionChanged += (s, e) => OnPropertyChanged(nameof(VisibleHostEntries));
        
        // Load data immediately
        LoadHostsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadHosts()
    {
        var data = await _hostsService.LoadHostsAsync();
        
        HostEntries.Clear();
        foreach (var item in data)
        {
            HostEntries.Add(item);
        }
    }

    [RelayCommand]
    private void AddEntry()
    {
        var newEntry = new HostEntry
        {
            IsActive = true,
            IpAddress = "127.0.0.1",
            HostNames = "new-site.local",
            LineNumber = 0 // New entry
        };
        HostEntries.Add(newEntry);
        SelectedEntry = newEntry;
        
        EntryAdded?.Invoke(newEntry);
    }



    [RelayCommand]
    private void RemoveEntry()
    {
        if (SelectedEntry != null)
        {
            HostEntries.Remove(SelectedEntry);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            await _hostsService.SaveHostsAsync(HostEntries);
            // In a real app, use a DialogService to show success
            Console.WriteLine("Saved successfully!"); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Preview()
    {
        // Generate the content string
        var content = _hostsService.GeneratePreview(HostEntries);
        
        // Trigger the event so the View can open the window
        PreviewRequested?.Invoke(content);
    }
}