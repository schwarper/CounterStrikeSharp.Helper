using System.Globalization;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

namespace CounterStrikeSharp.Helper.Internal;

internal static class Lang
{
    private static readonly string _path =
        $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/shared/CounterStrikeSharp.Helper/lang/";

    private static readonly Dictionary<string, Dictionary<string, string>> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    internal static string Localizer(this CCSPlayerController player, string key, params object[] args)
    {
        CultureInfo playerLanguage = player.GetLanguage();
        string langCode = playerLanguage.TwoLetterISOLanguageName;

        Dictionary<string, string> dict = LoadLanguage(langCode);

        if (!dict.TryGetValue(key, out string? value))
            return key;

        try
        {
            return string.Format(value, args);
        }
        catch
        {
            return value;
        }
    }

    private static Dictionary<string, string> LoadLanguage(string langCode)
    {
        if (_cache.TryGetValue(langCode, out Dictionary<string, string>? cached))
            return cached;

        string filePath = Path.Combine(_path, $"{langCode}.json");

        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(_path, "en.json");
            if (!File.Exists(filePath))
                return [];
        }

        string json = File.ReadAllText(filePath);
        Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ??
                   [];

        _cache[langCode] = dict;
        return dict;
    }
}
