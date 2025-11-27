using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace Manager.GUI.Services.Nginx;

public class NginxRegistryOptions : IRegistryOptions
{
    // 1. Point this to your EMBEDDED RESOURCE path
    // Format: AssemblyName.Folder.FileName
    private const string GrammarPath = "Manager.GUI.Resources.Grammars.nginx.json";

    public IRawGrammar GetGrammar(string scopeName)
    {
        if (scopeName == "source.nginx")
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GrammarPath);

            if (stream != null)
            {
                // FIX: Wrap the stream in a StreamReader
                using (var reader = new StreamReader(stream))
                {
                    return GrammarReader.ReadGrammarSync(reader);
                }
            }
        }

        return null;
    }

    public IRawTheme GetTheme(string scopeName)
    {
        // 1. Get the stream from the TextMateSharp assembly
        var stream = typeof(RegistryOptions).Assembly.GetManifestResourceStream("TextMateSharp.Grammars.Resources.Themes.dark_vs.json");

        // 2. Wrap it in a StreamReader
        if (stream != null)
        {
            using (var reader = new StreamReader(stream))
            {
                return ThemeReader.ReadThemeSync(reader);
            }
        }

        return null;
    }

    public IRawTheme GetDefaultTheme() => GetTheme("dark_vs");

    public ICollection<string> GetInjections(string scopeName) => null;
}