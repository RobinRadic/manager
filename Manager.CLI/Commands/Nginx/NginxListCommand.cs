using System.ComponentModel;
using Manager.Core.Features.Nginx;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Manager.CLI.Commands.Nginx;

public class NginxListCommand : AsyncCommand<NginxListCommand.Settings>
{
    private readonly NginxService _service;

    public class Settings : CommandSettings 
    {
        [CommandOption("-e|--enabled")]
        [Description("Show only enabled")]
        public bool ShowOnlyEnabled { get; set; }

        [CommandOption("-d|--disabled")]
        [Description("Show only disabled")]
        public bool ShowOnlyDisabled { get; set; }

    }
    
    public NginxListCommand(NginxService service) => _service = service;

    public override async Task<int> ExecuteAsync(CommandContext context, NginxListCommand.Settings settings, CancellationToken cancellationToken)
    {
        try 
        {
            var sites = await _service.LoadSitesAsync();
            var table = new Table();
            table.AddColumn("Site Name");
            table.AddColumn("Status");

            foreach (var site in sites)
            {
                var status = site.IsEnabled ? "[green]Enabled[/]" : "[grey]Disabled[/]";
                table.AddRow(site.Name, status);
            }

            AnsiConsole.Write(table);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}