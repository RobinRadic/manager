using System.Text;

namespace Manager.Core.Features.Nginx.Configuration;

public class NginxDirective : NginxNode
{
    public string Name { get; set; }
    public List<string> Arguments { get; set; } = new List<string>();
    public bool IsBlock => Children != null;
    public List<NginxNode> Children { get; set; } // Null if simple directive, distinct list if block

    public string Value => string.Join(" ", Arguments);
    
    
    public NginxDirective(string name)
    {
        Name = name;
    }

    public void AddArgument(string arg)
    {
        Arguments.Add(arg);
    }

    public override void Print(StringBuilder sb, int indentLevel)
    {
        sb.Append(Indent(indentLevel));
        sb.Append(Name);

        if (Arguments.Count > 0)
        {
            foreach (var arg in Arguments)
            {
                sb.Append(" ");
                // Basic quoting logic for output safety
                if (arg.Contains(" ") || arg.Contains(";") || arg.Contains("{") || arg.Contains("}"))
                {
                    sb.Append($"\"{arg.Replace("\"", "\\\"")}\"");
                }
                else
                {
                    sb.Append(arg);
                }
            }
        }

        if (IsBlock)
        {
            sb.AppendLine(" {");
            foreach (var child in Children)
            {
                child.Print(sb, indentLevel + 1);
            }
            sb.AppendLine($"{Indent(indentLevel)}}}");
        }
        else
        {
            sb.AppendLine(";");
        }
    }
}