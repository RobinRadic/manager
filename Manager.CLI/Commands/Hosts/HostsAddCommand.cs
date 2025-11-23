using Manager.Core.Features.Hosts;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Manager.CLI.Commands.Hosts;

public class HostsAddCommand : AsyncCommand<HostsAddCommand.Settings>
{
    private readonly HostsService _service;

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<IP>")]
        public string Ip { get; set; } = "";

        [CommandArgument(1, "<HOSTNAME>")]
        public string Hostname { get; set; } = "";

        [CommandOption("-c|--comment")]
        public string Comment { get; set; } = "";
    }

    public HostsAddCommand(HostsService service) => _service = service;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        await _service.AddEntryAsync(settings.Ip, settings.Hostname, settings.Comment);
        AnsiConsole.MarkupLine($"[green]Successfully added {settings.Hostname} ({settings.Ip})[/]");
        return 0;
    }
}