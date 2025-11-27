namespace Manager.GUI.Services.Settings;

public class AppSettings
{
    // Default values
    public string AppTheme { get; set; } = "Dark"; // "Dark", "Light"
    public string EditorTheme { get; set; } = "dark_vs";
    public string EditorFontFamily { get; set; } = "Cascadia Code";
    public double EditorFontSize { get; set; } = 14.0;
    
    // AI Configuration
    public string AiProvider { get; set; } = "Google"; // OpenAI, Google, Anthropic
    public string AiApiKey { get; set; } = "";
    public string AiModel { get; set; } = "gemini-1.5-flash"; // Default model
}