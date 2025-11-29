using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace Manager.GUI.Services.Php;

public class PhpRegistryOptions : IRegistryOptions
{
    private const string GrammarPath = "Manager.GUI.Resources.Grammars.ini.json";

    public IRawGrammar GetGrammar(string scopeName)
    {
        if (scopeName == "source.ini")
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

    public IRawTheme GetTheme(string scopeName) => GetThemeByName("dark_vs");

    public IRawTheme GetThemeByName(string themeName)
    {
        string resourceName = $"TextMateSharp.Grammars.Resources.Themes.{themeName}.json";
        var stream = typeof(RegistryOptions).Assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            using (var reader = new StreamReader(stream))
                return ThemeReader.ReadThemeSync(reader);
        }
        return null;
    }

    public List<string> GetAvailableThemes()
    {
        var themes = new List<string>();
        var resources = typeof(RegistryOptions).Assembly.GetManifestResourceNames();
        foreach (var res in resources)
        {
            if (res.StartsWith("TextMateSharp.Grammars.Resources.Themes.") && res.EndsWith(".json"))
            {
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