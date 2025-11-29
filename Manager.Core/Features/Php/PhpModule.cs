namespace Manager.Core.Features.Php;

public class PhpModule
{
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Sapi { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }

    public string Status => IsEnabled ? "Enabled" : "Disabled";
    public string DisplayName => $"{Name} ({Sapi})";
}