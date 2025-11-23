using System.Text;

namespace Manager.Core.Features.Nginx.Configuration;

public class NginxComment : NginxNode
{
    public string Text { get; set; }

    public NginxComment(string text)
    {
        Text = text;
    }

    public override string ToString() => $"# {Text}";

    public override void Print(StringBuilder sb, int indentLevel)
    {
        sb.AppendLine($"{Indent(indentLevel)}#{Text}");
    }
}