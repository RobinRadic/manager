using System.Net;
using System.Text;

namespace Manager.Core.Features.Hosts;

public class HostsService
{
    private readonly string _path = Environment.OSVersion.Platform == PlatformID.Unix
        ? "/etc/hosts"
        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");

    public async Task<List<HostEntry>> LoadHostsAsync()
    {
        // ... (Your existing parsing logic matches here, just wrap in Task.Run or use File.ReadAllLinesAsync) ...
        // For brevity, using your logic but ensuring async signature
        if (!File.Exists(_path)) return new List<HostEntry>();

        var lines = await File.ReadAllLinesAsync(_path);
        var entries = new List<HostEntry>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) 
            {
                entries.Add(new HostEntry { LineNumber = i + 1, IsBlank = true });
                continue;
            }

            var trimmedLine = line.Trim();
            var entry = new HostEntry { LineNumber = i + 1, IsActive = !trimmedLine.StartsWith("#") };
            var parseableLine = trimmedLine.TrimStart('#').Trim();

            // Extract inline comments
            string contentPart;
            string commentPart = string.Empty;
            int commentIndex = parseableLine.IndexOf('#');
            
            if (commentIndex != -1)
            {
                contentPart = parseableLine.Substring(0, commentIndex).Trim();
                commentPart = parseableLine.Substring(commentIndex + 1).Trim();
            }
            else
            {
                contentPart = parseableLine;
            }

            var parts = contentPart.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0 && IPAddress.TryParse(parts[0], out _))
            {
                entry.IpAddress = parts[0];
                if (parts.Length > 1) entry.HostNames = string.Join(" ", parts.Skip(1));
                entry.Comment = commentPart;
            }
            else
            {
                // It's a pure comment line
                entry.IsActive = false; // Comments are technically "inactive"
                entry.Comment = string.IsNullOrWhiteSpace(commentPart) ? parseableLine : commentPart;
            }
            entries.Add(entry);
        }
        return entries;
    }

    public async Task AddEntryAsync(string ip, string hostname, string comment = "")
    {
        var entries = await LoadHostsAsync();
        
        // Check for duplicates
        if (entries.Any(e => e.HostNames.Split(' ').Contains(hostname) && e.IsActive))
        {
            throw new Exception($"Host '{hostname}' already exists.");
        }

        entries.Add(new HostEntry
        {
            IpAddress = ip,
            HostNames = hostname,
            Comment = comment,
            IsActive = true,
            LineNumber = entries.Count + 1
        });

        await SaveHostsAsync(entries);
    }

    public async Task RemoveEntryAsync(string hostname)
    {
        var entries = await LoadHostsAsync();
        var target = entries.FirstOrDefault(e => e.HostNames.Contains(hostname) && !e.IsPureComment);
        
        if (target == null) throw new Exception($"Host '{hostname}' not found.");

        entries.Remove(target);
        await SaveHostsAsync(entries);
    }

    public async Task ToggleEntryAsync(string hostname, bool enable)
    {
        var entries = await LoadHostsAsync();
        // Find entry (ignoring blank lines and pure comments)
        var target = entries.FirstOrDefault(e => e.HostNames.Contains(hostname) && !e.IsPureComment);

        if (target == null) throw new Exception($"Host '{hostname}' not found.");

        if (target.IsActive == enable) return; // No change needed

        target.IsActive = enable;
        await SaveHostsAsync(entries);
    }
    
    public string GeneratePreview(IEnumerable<HostEntry> entries)
    {
        var sb = new StringBuilder();
        foreach (var entry in entries)
        {
            sb.AppendLine(entry.ToString());
        }
        return sb.ToString();
    }
    
    public async Task SaveHostsAsync(IEnumerable<HostEntry> entries)
    {
        var sb = new StringBuilder();
        foreach (var entry in entries) sb.AppendLine(entry.ToString());

        return; // temporary @todo remove
        var backupPath = _path + ".bak";

        try 
        {
            // 1. Create Backup
            if (File.Exists(_path)) File.Copy(_path, backupPath, overwrite: true);

            // 2. Write
            await File.WriteAllTextAsync(_path, sb.ToString());

            // 3. Delete Backup on success
            File.Delete(backupPath);
        }
        catch
        {
            // Restore backup if something failed
            if (File.Exists(backupPath)) File.Move(backupPath, _path, overwrite: true);
            throw;
        }
    }
}