using Manager.Core.Features.Hosts;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Manager.CLI.Commands.Hosts;

public class HostsToggleCommand : AsyncCommand<HostsToggleCommand.Settings>
{
    private readonly HostsService _service;
    private readonly bool _enable;

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<HOSTNAME>")]
        public string Hostname { get; set; } = "";
    }

    // Constructor Injection + State
    public HostsToggleCommand(HostsService service, bool enable)
    {
        _service = service;
        _enable = enable;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        await _service.ToggleEntryAsync(settings.Hostname, _enable);
        var action = _enable ? "enabled" : "disabled";
        AnsiConsole.MarkupLine($"[green]Successfully {action} {settings.Hostname}[/]");
        return 0;
    }
}