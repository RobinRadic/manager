using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace Manager.GUI.Services.Settings;

public class SettingsService
{
    private const string SettingsFileName = "settings.json";
    private readonly string _filePath;

    public AppSettings CurrentSettings { get; private set; }

    // Event to notify other parts of the app when settings change
    public event Action<AppSettings>? SettingsChanged;

    public SettingsService()
    {
        // Save in ApplicationData or right next to the executable
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(folder, "ManagerApp");
        Directory.CreateDirectory(appFolder);
        _filePath = Path.Combine(appFolder, SettingsFileName);

        CurrentSettings = LoadSettings();
    }

    private AppSettings LoadSettings()
    {
        if (!File.Exists(_filePath))
        {
            return new AppSettings();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            CurrentSettings = settings;
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
            
            // Notify listeners
            SettingsChanged?.Invoke(CurrentSettings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}