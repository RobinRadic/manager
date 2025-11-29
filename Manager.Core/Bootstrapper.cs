using Manager.Core.Features.Hosts;
using Manager.Core.Features.Nginx;
using Manager.Core.Features.Php;
using Microsoft.Extensions.DependencyInjection;

namespace Manager.Core;

public static class Bootstrapper
{
    public static IServiceCollection RegisterManagers(this IServiceCollection services)
    {
        services.AddSingleton<HostsService>();
        services.AddSingleton<NginxService>();
        services.AddSingleton<PhpService>();
        
        // services.AddSingleton<INginxService, NginxService>();
        return services;
    }
}