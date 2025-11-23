namespace Manager.Core.Features.Hosts;
public class HostEntry
{
    public int LineNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsBlank { get; set; } = false;
    
    // Check if this line is just a comment (not a disabled host)
    public bool IsPureComment => string.IsNullOrWhiteSpace(IpAddress) && !string.IsNullOrWhiteSpace(Comment);

    public string IpAddress { get; set; } = string.Empty;
    public string HostNames { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;

    public override string ToString()
    {
        if (IsBlank) return string.Empty;

        // Case 1: Pure comment line (e.g. "# This is a section header")
        if (IsPureComment) return $"# {Comment}";

        // Case 2: Standard Host Entry
        var prefix = IsActive ? "" : "# ";
        var commentPart = string.IsNullOrWhiteSpace(Comment) ? "" : $" # {Comment}";
        
        return $"{prefix}{IpAddress}\t{HostNames}{commentPart}";
    }
}