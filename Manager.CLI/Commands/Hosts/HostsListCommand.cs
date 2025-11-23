using Manager.Core.Features.Hosts;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Manager.CLI.Commands.Hosts;

public class HostsListCommand : AsyncCommand<HostsListCommand.Settings>
{
    private readonly HostsService _service;

    public class Settings : CommandSettings 
    {
        [CommandOption("-a|--all")]
        [Description("Show comments and blank lines")]
        public bool ShowAll { get; set; }
    }

    public HostsListCommand(HostsService service) => _service = service;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var entries = await _service.LoadHostsAsync();
        var table = new Table();

        // table.AddColumn("Status");
        table.AddColumn("IP Address");
        table.AddColumn("Hostnames");
        table.AddColumn("Comment");

        foreach (var entry in entries)
        {
            if (entry.IsBlank && !settings.ShowAll) continue;
            if (entry.IsPureComment && !settings.ShowAll) continue;

            if (entry.IsPureComment)
            {
                table.AddRow("[grey]#[/]", "-", "-", $"[grey]{entry.Comment}[/]");
            }
            else if (!entry.IsBlank)
            {
                var color = entry.IsActive ? "green" : "grey";
                var status = entry.IsActive ? "Active" : "Disabled";
                
                table.AddRow(
                    // $"[{color}]{status}[/]", 
                    $"[{color}]{entry.IpAddress}[/]", 
                    $"[{color}]{entry.HostNames}[/]", 
                    $"[grey]{entry.Comment}[/]"
                );
            }
        }

        AnsiConsole.Write(table);
        return 0;
    }

}