using Manager.Core.Features.Nginx.Configuration;

namespace Manager.Core.Features.Nginx;

public class NginxSite
{
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }

    // Helper to determine status color/icon in UI
    public string Status => IsEnabled ? "Active" : "Disabled";

    public async Task<string> GetSource()
    {
        return await File.ReadAllTextAsync(FilePath);
    }
    
    public async Task<NginxConfig> GetConfig()
    {
        var source = await GetSource();
        return NginxParser.fromSource(source);
    }
}