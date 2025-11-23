using Manager.Core.Features.Hosts;
using Manager.Core.Features.Nginx;
using Microsoft.Extensions.DependencyInjection;

namespace Manager.Core;

public static class Bootstrapper
{
    public static IServiceCollection RegisterManagers(this IServiceCollection services)
    {
        services.AddSingleton<HostsService>();
        services.AddSingleton<NginxService>();
        
        // services.AddSingleton<INginxService, NginxService>();
        return services;
    }
}