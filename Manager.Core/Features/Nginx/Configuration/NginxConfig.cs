using System.Text;

namespace Manager.Core.Features.Nginx.Configuration;

public class NginxConfig
{
    public List<NginxNode> RootNodes { get; set; } = new List<NginxNode>();

    public NginxDirective this[string path]
    {
        get
        {
            if (string.IsNullOrEmpty(path)) return null;

            var segments = path.Split(':');
            List<NginxNode> currentScope = RootNodes;
            NginxDirective currentDirective = null;

            foreach (var segment in segments)
            {
                // 1. Parse Name and Index (e.g., "server[0]" or just "server")
                string name = segment;
                int index = 0;

                if (segment.Contains("[") && segment.EndsWith("]"))
                {
                    int openIndex = segment.IndexOf('[');
                    name = segment.Substring(0, openIndex);
                    string indexStr = segment.Substring(openIndex + 1, segment.Length - openIndex - 2);
                    int.TryParse(indexStr, out index);
                }

                // 2. Find matching directives in the current scope
                // We only look at Directives, skipping comments
                var matches = currentScope.OfType<NginxDirective>().Where(d => d.Name == name);

                // 3. Select the Nth match
                currentDirective = matches.Skip(index).FirstOrDefault();

                // If not found at any stage, return null immediately
                if (currentDirective == null) return null;

                // 4. Prepare scope for next iteration
                if (currentDirective.IsBlock)
                {
                    currentScope = currentDirective.Children;
                }
                else
                {
                    // If we are at a leaf node but there are more segments requested, it's a mismatch
                    // unless this is the very last segment loop, in which case we break successfully.
                    if (segment != segments.Last())
                        return null;
                }
            }

            return currentDirective;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var node in RootNodes)
        {
            node.Print(sb, 0);
        }

        return sb.ToString();
    }

    // Helper to find specific directives (e.g., all "server" blocks)
    public IEnumerable<NginxDirective> FindDirectives(string name)
    {
        return RootNodes.OfType<NginxDirective>().Where(d => d.Name == name);
    }
}