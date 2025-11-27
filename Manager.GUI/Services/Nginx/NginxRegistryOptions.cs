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
        // Default theme fallback
        return GetThemeByName("dark_vs");
    }

    public IRawTheme GetThemeByName(string themeName)
    {
        // TextMateSharp stores themes as embedded resources in this namespace format
        string resourceName = $"TextMateSharp.Grammars.Resources.Themes.{themeName}.json";
        
        var stream = typeof(RegistryOptions).Assembly.GetManifestResourceStream(resourceName);

        if (stream != null)
        {
            using (var reader = new StreamReader(stream))
            {
                return ThemeReader.ReadThemeSync(reader);
            }
        }

        return null;
    }

    public List<string> GetAvailableThemes()
    {
        var themes = new List<string>();
        // Scan the TextMateSharp assembly for theme resources
        var resources = typeof(RegistryOptions).Assembly.GetManifestResourceNames();
        
        foreach (var res in resources)
        {
            if (res.StartsWith("TextMateSharp.Grammars.Resources.Themes.") && res.EndsWith(".json"))
            {
                // Extract "dark_vs" from "TextMateSharp.Grammars.Resources.Themes.dark_vs.json"
                var name = res.Replace("TextMateSharp.Grammars.Resources.Themes.", "").Replace(".json", "");
                themes.Add(name);
            }
        }
        
        themes.Sort();
        return themes;
    }

    public IRawTheme GetDefaultTheme() => GetTheme("dark_vs");

    public ICollection<string> GetInjections(string scopeName) => null;
}