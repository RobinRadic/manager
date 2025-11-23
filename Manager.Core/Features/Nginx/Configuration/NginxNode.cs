using System.Text;

namespace Manager.Core.Features.Nginx.Configuration;

public abstract class NginxNode
{
    // Base class for all config elements
    public abstract void Print(StringBuilder sb, int indentLevel);

    protected string Indent(int level)
    {
        return new string(' ', level * 4);
    }
}