using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CCSGameRulesProxy"/> and <see cref="CCSGameRules"/>class.
/// </summary>
public static class GameRulesExtensions
{
    /// <summary>
    /// Extends the current round time by the specified number of minutes.
    /// </summary>
    /// <param name="gameRulesProxy">The game rules proxy.</param>
    /// <param name="minutes">The number of minutes to add to the round time.</param>
    /// <remarks>Credits to T3Marius for the original implementation.</remarks>
    public static void ExtendRoundTime(this CCSGameRulesProxy gameRulesProxy, int minutes)
    {
        if (gameRulesProxy.GameRules is not { } gameRules)
            return;

        gameRules.RoundTime += minutes * 60;
        Utilities.SetStateChanged(gameRulesProxy, "CCSGameRules", "m_iRoundTime");
    }

    /// <summary>
    /// Gets the remaining round time in seconds.
    /// </summary>
    /// <param name="gameRules">The <see cref="CCSGameRules"/> instance.</param>
    /// <returns> The remaining time in seconds before the round ends. </returns>
    /// <remarks>Credits to imi-tat0r for the original implementation.</remarks>
    public static float GetRemainingRoundTime(this CCSGameRules gameRules)
    {
        return gameRules.RoundStartTime + gameRules.RoundTime - Server.CurrentTime;
    }
}