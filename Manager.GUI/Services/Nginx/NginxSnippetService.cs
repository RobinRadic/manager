using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manager.GUI.Services.Nginx;

public class NginxSnippetService
{
    private const string FileName = "nginx_snippets.json";
    private readonly string _filePath;

    public NginxSnippetService()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(folder, "ManagerApp");
        Directory.CreateDirectory(appFolder);
        _filePath = Path.Combine(appFolder, FileName);
    }

    public async Task<List<NginxSnippet>> LoadSnippetsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return GetDefaultSnippets();
        }

        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<NginxSnippet>>(json) ?? GetDefaultSnippets();
        }
        catch
        {
            return GetDefaultSnippets();
        }
    }

    public async Task SaveSnippetsAsync(IEnumerable<NginxSnippet> snippets)
    {
        try
        {
            var json = JsonSerializer.Serialize(snippets, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving snippets: {ex.Message}");
        }
    }

    private List<NginxSnippet> GetDefaultSnippets()
    {
        return new List<NginxSnippet>
        {
            new NginxSnippet 
            { 
                Name = "Reverse Proxy", 
                Description = "Basic proxy_pass configuration",
                Content = "location / {\n    proxy_pass http://localhost:5000;\n    proxy_http_version 1.1;\n    proxy_set_header Upgrade $http_upgrade;\n    proxy_set_header Connection keep-alive;\n    proxy_set_header Host $host;\n    proxy_cache_bypass $http_upgrade;\n    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;\n    proxy_set_header X-Forwarded-Proto $scheme;\n}" 
            },
            new NginxSnippet 
            { 
                Name = "CORS Headers", 
                Description = "Allow Cross-Origin requests",
                Content = "add_header 'Access-Control-Allow-Origin' '*';\nadd_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS';\nadd_header 'Access-Control-Allow-Headers' 'DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range';\nadd_header 'Access-Control-Expose-Headers' 'Content-Length,Content-Range';" 
            },
            new NginxSnippet 
            { 
                Name = "SPA Fallback", 
                Description = "For React/Angular/Vue apps",
                Content = "location / {\n    try_files $uri $uri/ /index.html;\n}" 
            },
            new NginxSnippet 
            { 
                Name = "SSL Redirect", 
                Description = "Redirect HTTP to HTTPS",
                Content = "server {\n    listen 80;\n    server_name example.com;\n    return 301 https://$host$request_uri;\n}" 
            }
        };
    }
}