using CliWrap;
using CliWrap.Buffered;

namespace Manager.Core.Features.Nginx;

public class NginxService
{
    private const string AvailablePath = "/etc/nginx/sites-available";
    private const string EnabledPath = "/etc/nginx/sites-enabled";

    // 1. List Sites
    public Task<List<NginxSite>> LoadSitesAsync()
    {
        var sites = new List<NginxSite>();
        
        if (!Directory.Exists(AvailablePath)) 
            return Task.FromResult(sites); // Or throw error if Nginx isn't installed

        var files = Directory.GetFiles(AvailablePath);
        
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var enabledLink = Path.Combine(EnabledPath, name);
            
            sites.Add(new NginxSite
            {
                Name = name,
                FilePath = file,
                // It is enabled if a symlink exists in the enabled folder
                IsEnabled = File.Exists(enabledLink) 
            });
        }

        return Task.FromResult(sites);
    }

    // 2. Toggle (Enable/Disable)
    public async Task ToggleSiteAsync(NginxSite site, bool enable)
    {
        var linkPath = Path.Combine(EnabledPath, site.Name);

        if (enable)
        {
            if (File.Exists(linkPath)) return; // Already enabled

            // Create Symlink: ln -s /etc/nginx/sites-available/site /etc/nginx/sites-enabled/site
            await Cli.Wrap("ln")
                .WithArguments(new[] { "-s", site.FilePath, linkPath })
                .ExecuteAsync();
        }
        else
        {
            if (!File.Exists(linkPath)) return; // Already disabled

            // Remove Symlink: rm /etc/nginx/sites-enabled/site
            await Cli.Wrap("rm")
                .WithArguments(linkPath)
                .ExecuteAsync();
        }

        // Always reload Nginx to apply changes
        await ReloadNginxAsync();
    }

    // 3. Get Config Content
    public async Task<string> GetConfigAsync(string siteName)
    {
        var path = Path.Combine(AvailablePath, siteName);
        if (!File.Exists(path)) throw new FileNotFoundException("Site config not found", path);
        return await File.ReadAllTextAsync(path);
    }

    // 4. Safe Save (The most critical method)
    public async Task SaveConfigAsync(string siteName, string newContent)
    {
        var path = Path.Combine(AvailablePath, siteName);
        var backupPath = path + ".bak";

        // A. Create Backup
        if (File.Exists(path)) File.Copy(path, backupPath, overwrite: true);

        try
        {
            // B. Write New Content
            await File.WriteAllTextAsync(path, newContent);

            // C. Validate Config (nginx -t)
            var validation = await Cli.Wrap("nginx")
                .WithArguments("-t")
                .ExecuteBufferedAsync();

            if (validation.ExitCode != 0)
            {
                throw new Exception($"Nginx Config Error:\n{validation.StandardError}");
            }

            // D. Reload Nginx if valid
            await ReloadNginxAsync();

            // E. Cleanup Backup
            File.Delete(backupPath);
        }
        catch
        {
            // ROLLBACK on any error (file IO or Nginx validation)
            if (File.Exists(backupPath)) File.Move(backupPath, path, overwrite: true);
            throw; // Re-throw so UI can display the error
        }
    }

    private async Task ReloadNginxAsync()
    {
        await Cli.Wrap("systemctl")
            .WithArguments("reload nginx")
            .ExecuteAsync();
    }
}