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
    
    public async Task<List<NginxConfigFile>> LoadConfigFilesAsync()
    {
        return await Task.Run(() =>
        {
            var list = new List<NginxConfigFile>();
            var rootPath = "/etc/nginx";

            if (!Directory.Exists(rootPath)) return list;

            // 1. Get root files (nginx.conf, mime.types, etc.)
            var rootFiles = Directory.GetFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".conf") || f.EndsWith(".types") || f.EndsWith("_params"));

            foreach (var file in rootFiles)
            {
                list.Add(new NginxConfigFile 
                { 
                    Name = Path.GetFileName(file), 
                    FilePath = file 
                });
            }

            // 2. Get conf.d files if exists
            var confD = Path.Combine(rootPath, "conf.d");
            if (Directory.Exists(confD))
            {
                var confDFiles = Directory.GetFiles(confD, "*.conf");
                foreach (var file in confDFiles)
                {
                    list.Add(new NginxConfigFile 
                    { 
                        Name = $"conf.d/{Path.GetFileName(file)}", 
                        FilePath = file 
                    });
                }
            }

            return list.OrderBy(x => x.Name).ToList();
        });
    }

// Generic file saver
    public async Task SaveFileAsync(string path, string content)
    {
        await Task.Run(() =>
        {
            // Create backup
            if (File.Exists(path))
            {
                File.Copy(path, path + ".bak", true);
            }
        
            File.WriteAllText(path, content);
        
            // Validate Nginx (optional, but recommended)
            // RunProcess("nginx", "-t"); 
        
            // Reload Nginx
            // RunProcess("systemctl", "reload nginx");
        });
    }
}