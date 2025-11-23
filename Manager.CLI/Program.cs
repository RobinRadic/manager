using Manager.CLI.Commands;
using Manager.CLI.Commands.Hosts;
using Manager.CLI.Commands.Nginx;
using Manager.CLI.Infrastructure;
using Manager.Core;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

// 1. Setup DI
var serviceCollection = new ServiceCollection();
serviceCollection.RegisterManagers(); // From Core

// 2. Create a TypeRegistrar for Spectre.Console to use our DI container
var registrar = new TypeRegistrar(serviceCollection);

// 3. Setup the App
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("manager");
    config.AddCommand<TestCommand>("test").WithDescription("Test command");
    
    config.AddBranch("hosts", hosts =>
    {
        hosts.AddCommand<HostsListCommand>("list").WithDescription("List all host entries");
        hosts.AddCommand<HostsAddCommand>("add").WithDescription("Add a new host entry");
        hosts.AddCommand<HostsRemoveCommand>("remove").WithDescription("Remove a host entry");
        hosts.AddCommand<HostsEnableCommand>("enable").WithDescription("Uncomment a host entry");
        hosts.AddCommand<HostsDisableCommand>("disable").WithDescription("Comment out a host entry");
    });
    config.AddBranch("nginx", nginx =>
    {
        nginx.AddCommand<NginxListCommand>("list").WithDescription("List all nginx entries");
    });
    // We will add commands here later, e.g.:
    // config.AddCommand<NginxListCommand>("nginx");
});

return app.Run(args);