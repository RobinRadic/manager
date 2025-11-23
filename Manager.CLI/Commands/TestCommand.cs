using System.ComponentModel;
using Manager.CLI.Commands.Nginx;
using Manager.Core.Features.Nginx;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Manager.CLI.Commands;

public class TestCommand  : AsyncCommand<TestCommand.Settings>
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
    
    public TestCommand(NginxService service) => _service = service;

    public override async Task<int> ExecuteAsync(CommandContext context, TestCommand.Settings settings, CancellationToken cancellationToken)
    {

        var sites = await _service.LoadSitesAsync();

        var site = sites.First();

        var config = await site.GetConfig();
        
        

        return 0;
    }
}