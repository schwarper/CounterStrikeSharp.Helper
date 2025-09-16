using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CCSGameRulesProxy"/> and <see cref="CCSGameRules"/>class.
/// </summary>
public static class GameRulesExtensions
{
    private static readonly ConVar mp_halftime;
    private static readonly ConVar mp_maxrounds;
    private static readonly MemoryFunctionWithReturn<CCSGameRules, nint> CCSGameRules__CleanUpMap;

    static GameRulesExtensions()
    {
        mp_halftime = ConVar.Find("mp_halftime")!;
        mp_maxrounds = ConVar.Find("mp_maxrounds")!;

        CCSGameRules__CleanUpMap = new(GameData.GetSignature("CCSGameRules__CleanUpMap"));
    }

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

    /// <summary>
    /// Determines whether the current round is a pistol round.
    /// </summary>
    /// <param name="gameRules">The <see cref="CCSGameRules"/> instance.</param>
    /// <returns>
    /// <see langword="true"/> if the current round is a pistol round;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPistolRound(this CCSGameRules gameRules)
    {
        bool isHalftime = mp_halftime.GetPrimitiveValue<bool>();
        int maxRounds = mp_maxrounds.GetPrimitiveValue<int>();

        return gameRules.TotalRoundsPlayed == 0 ||
               (isHalftime && maxRounds / 2 == gameRules.TotalRoundsPlayed) ||
               gameRules.GameRestart;
    }

    /// <summary>
    /// Forces a full cleanup
    /// </summary>
    /// <param name="gameRules">The <see cref="CCSGameRules"/> instance.</param>
    /// <remarks>Credits to xstage for the original implementation.</remarks>
    public static void CleanUpMap(this CCSGameRules gameRules)
    {
        CCSGameRules__CleanUpMap.Invoke(gameRules);
    }

}