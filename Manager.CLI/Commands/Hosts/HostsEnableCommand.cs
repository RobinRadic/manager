using Manager.Core.Features.Hosts;

namespace Manager.CLI.Commands.Hosts;

public class HostsEnableCommand : HostsToggleCommand 
{ 
    public HostsEnableCommand(HostsService s) : base(s, true) {} 
}