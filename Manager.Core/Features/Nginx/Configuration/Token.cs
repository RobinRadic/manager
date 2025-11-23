namespace Manager.Core.Features.Nginx.Configuration;

internal class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
}