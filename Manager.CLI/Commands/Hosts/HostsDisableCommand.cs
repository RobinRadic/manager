using Manager.Core.Features.Hosts;

namespace Manager.CLI.Commands.Hosts;

public class HostsDisableCommand : HostsToggleCommand
{
    public HostsDisableCommand(HostsService s) : base(s, false)
    {
    }
}