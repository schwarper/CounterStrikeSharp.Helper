using System.Xml.Linq;

namespace CounterStrikeSharp.Helper.Utils;

/// <summary>
/// Provides utility methods for working with SteamIDs.
/// </summary>
public static class SteamIDUtils
{
    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// Attempts to parse a string as a valid 64-bit SteamID.
    /// </summary>
    /// <param name="id">The string to parse, which should represent a 17-digit SteamID.</param>
    /// <param name="steamId">When this method returns, contains the 64-bit SteamID if the parsing was successful; otherwise, it is zero.</param>
    /// <returns>
    /// <c>true</c> if the string was successfully parsed as a valid SteamID; otherwise, <c>false</c>.
    /// </returns>
    public static bool SteamIDTryParse(string id, out ulong steamId)
    {
        steamId = 0;

        if (id.Length != 17)
        {
            return false;
        }

        if (!ulong.TryParse(id, out steamId))
        {
            return false;
        }

        const ulong minSteamID = 76561197960265728;
        return steamId >= minSteamID;
    }

    /// <summary>
    /// Retrieves the player's name associated with a given SteamID by querying the Steam Community profile.
    /// </summary>
    /// <param name="steamID">The 64-bit SteamID of the player.</param>
    /// <remarks>Credits to deana for the original implementation.</remarks>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task's result is the player's name as a string, or the SteamID as a string if the name could not be retrieved.</returns>
    public static async Task<string> GetPlayerNameFromSteamID(ulong steamID)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"https://steamcommunity.com/profiles/{steamID}/?xml=1");
            response.EnsureSuccessStatusCode();

            string xmlContent = await response.Content.ReadAsStringAsync();
            XDocument doc = XDocument.Parse(xmlContent);
            string? name = doc.Root?.Element("steamID")?.Value.Trim();

            return string.IsNullOrWhiteSpace(name) ? steamID.ToString() : name;
        }
        catch
        {
            return steamID.ToString();
        }
    }
}