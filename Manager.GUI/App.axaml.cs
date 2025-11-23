using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using Manager.Core;
using Manager.GUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Manager.GUI;

public class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        var _ = typeof(TextEditor);

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.RegisterManagers();

        collection.AddTransient<MainViewModel>();
        collection.AddTransient<HostsViewModel>();
        collection.AddTransient<NginxViewModel>();
        collection.AddTransient<SettingsViewModel>();

        Services = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = Services.GetRequiredService<MainViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}