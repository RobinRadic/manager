using CliWrap;
using CliWrap.Buffered;
using System.Text.RegularExpressions;

namespace Manager.Core.Features.Php;

public class PhpService
{
    private const string BasePath = "/etc/php";

    // Get list of installed PHP versions (folders in /etc/php)
    public List<string> GetInstalledVersions()
    {
        if (!Directory.Exists(BasePath)) return new List<string>();

        var dirs = Directory.GetDirectories(BasePath);
        var versions = new List<string>();

        foreach (var dir in dirs)
        {
            var folderName = Path.GetFileName(dir);
            // Matches 7.4, 8.0, 8.1 etc.
            if (Regex.IsMatch(folderName, @"^\d+\.\d+$"))
            {
                versions.Add(folderName);
            }
        }

        return versions.OrderByDescending(v => v).ToList();
    }

    public async Task<List<PhpModule>> GetModulesAsync(string version)
    {
        var modules = new List<PhpModule>();
        var versionPath = Path.Combine(BasePath, version);
        var modsAvailablePath = Path.Combine(versionPath, "mods-available");

        if (!Directory.Exists(modsAvailablePath)) return modules;

        // Dynamically find SAPIs by looking at folders in /etc/php/{version}/
        // Common SAPIs: cli, fpm, cgi, apache2, embed, phpdbg
        // We exclude 'mods-available' to get the list of SAPIs
        var sapis = Directory.GetDirectories(versionPath)
            .Select(d => Path.GetFileName(d))
            .Where(name => !name.Equals("mods-available", StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Get all available module definitions
        var moduleFiles = Directory.GetFiles(modsAvailablePath, "*.ini");

        foreach (var sapi in sapis)
        {
            var sapiConfDir = Path.Combine(versionPath, sapi, "conf.d");
            bool sapiConfExists = Directory.Exists(sapiConfDir);

            foreach (var file in moduleFiles)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                
                // Check enablement: Look for any symlink in {sapi}/conf.d that points to this module
                // Usually named like 20-json.ini
                bool isEnabled = false;
                
                if (sapiConfExists)
                {
                    // We check if a file ending in "-{name}.ini" exists in the conf.d folder
                    var confFiles = Directory.GetFiles(sapiConfDir, $"*-{name}.ini");
                    isEnabled = confFiles.Any();
                }

                modules.Add(new PhpModule
                {
                    Name = name,
                    FilePath = file,
                    Version = version,
                    Sapi = sapi,
                    IsEnabled = isEnabled
                });
            }
        }

        // Return ordered by SAPI then Module Name for better UI grouping
        return modules.OrderBy(m => m.Sapi).ThenBy(m => m.Name).ToList();
    }

    public async Task<string> GetConfigContentAsync(string filePath)
    {
        if (!File.Exists(filePath)) return string.Empty;
        return await File.ReadAllTextAsync(filePath);
    }

    public async Task SaveConfigAsync(string filePath, string content)
    {
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task ToggleModuleAsync(PhpModule module, bool enable)
    {
        // Use phpenmod / phpdismod
        // Syntax: phpenmod -v <version> -s <sapi> <module_name>
        
        var cmd = enable ? "phpenmod" : "phpdismod";
        
        // We now explicitly target the specific SAPI
        var args = new[] { "-v", module.Version, "-s", module.Sapi, module.Name };
        
        var result = await Cli.Wrap(cmd)
            .WithArguments(args)
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0)
        {
            throw new Exception($"Failed to {cmd} module {module.Name} for SAPI {module.Sapi}: {result.StandardError}");
        }
    }
}