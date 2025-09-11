using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.Helper.Internal;
using static CounterStrikeSharp.API.Modules.Commands.Targeting.Target;

namespace CounterStrikeSharp.Helper.Utils;

/// <summary>
/// Provides utility methods for processing target string.
/// </summary>
public static class TargetUtils
{
    /// <summary>
    /// Specifies filters for processing command targets.
    /// </summary>
    [Flags]
    public enum ProcessTargetFilterFlag
    {
        /// <summary>
        /// No filter applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only allow alive players as targets.
        /// </summary>
        FilterAlive = 1 << 0,

        /// <summary>
        /// Only allow dead players as targets.
        /// </summary>
        FilterDead = 1 << 1,

        /// <summary>
        /// Filter out targets that the command issuer cannot target due to immunity rules.
        /// </summary>
        FilterNoImmunity = 1 << 2,

        /// <summary>
        /// Do not allow multiple target patterns like @all, @ct, etc.
        /// </summary>
        FilterNoMulti = 1 << 3,

        /// <summary>
        /// Do not allow bots to be targeted.
        /// </summary>
        FilterNoBots = 1 << 4
    }

    /// <summary>
    /// Represents the result of a target processing operation.
    /// </summary>
    public enum ProcessTargetResultFlag
    {
        /// <summary>
        /// Target(s) were successfully found and filtered.
        /// </summary>
        TargetFound,

        /// <summary>
        /// No target was found matching the initial pattern.
        /// </summary>
        TargetNone,

        /// <summary>
        /// A single target was found, but they were not alive as required by the filter.
        /// Or a multi-target filter resulted in no alive players.
        /// </summary>
        TargetNotAlive,

        /// <summary>
        /// A single target was found, but they were not dead as required by the filter.
        /// Or a multi-target filter resulted in no dead players.
        /// </summary>
        TargetNotDead,

        /// <summary>
        /// The target is immune and cannot be targeted by the command issuer.
        /// </summary>
        TargetImmune,

        /// <summary>
        /// A multi-target filter (like @all) resulted in no players after filtering.
        /// </summary>
        TargetEmptyFilter,

        /// <summary>
        /// The target was found, but it was not a human player as required by the filter.
        /// </summary>
        TargetNotHuman,

        /// <summary>
        /// The target string was ambiguous and matched more than one player when a single target was expected.
        /// </summary>
        TargetAmbiguous
    }

    /// <summary>
    /// Processes a target string, finds matching players, and applies specified filters.
    /// </summary>
    /// <param name="player">The player who executed the command.</param>
    /// <param name="targetString">The target string (e.g., player name, #userid, @all).</param>
    /// <param name="filter">Flags to filter the found targets.</param>
    /// <param name="tnIsMl">If true, the target name buffer will be an ML phrase. Otherwise, it will be normal string.</param>
    /// <param name="targetname">
    /// An output list that will contain the resolved target names. These may be localization keys 
    /// (e.g., "all", "ct") if <paramref name="tnIsMl"/> is true, or actual player names otherwise.
    /// </param>
    /// <param name="players">An output list that will be populated with the player entities matching the target string.</param>

    public static ProcessTargetResultFlag ProcessTargetString(CCSPlayerController? player,
        string targetString, ProcessTargetFilterFlag filter, bool tnIsMl,
        out List<string> targetname, out List<CCSPlayerController> players)
    {
        targetname = [];
        players = new Target(targetString).GetTarget(player).Players;

        if (players.Count == 0)
        {
            return ProcessTargetResultFlag.TargetNone;
        }

        if (players.Count > 1 && filter.HasFlag(ProcessTargetFilterFlag.FilterNoMulti))
        {
            return ProcessTargetResultFlag.TargetAmbiguous;
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterNoImmunity))
        {
            players.RemoveAll(target => player != null && !AdminManager.CanPlayerTarget(player, target));
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetImmune;
            }
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterNoBots))
        {
            players.RemoveAll(p => p.IsBot);
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetNotHuman;
            }
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterAlive))
        {
            players.RemoveAll(p => p.PlayerPawn.Value?.LifeState != (byte)LifeState_t.LIFE_ALIVE);
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetNotAlive;
            }
        }

        if (filter.HasFlag(ProcessTargetFilterFlag.FilterDead))
        {
            players.RemoveAll(p => p.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE);
            if (players.Count == 0)
            {
                return ProcessTargetResultFlag.TargetNotDead;
            }
        }

        if (tnIsMl)
        {
            TargetTypeMap.TryGetValue(targetString, out TargetType type);
            string representativeName = type switch
            {
                TargetType.GroupAll => "all",
                TargetType.GroupBots => "bots",
                TargetType.GroupHumans => "humans",
                TargetType.GroupAlive => "alive",
                TargetType.GroupDead => "dead",
                TargetType.GroupNotMe => "notme",
                TargetType.PlayerMe => players[0].PlayerName,
                TargetType.TeamCt => "ct",
                TargetType.TeamT => "t",
                TargetType.TeamSpec => "spec",
                _ => players[0].PlayerName
            };
            targetname.Add(representativeName);
        }
        else
        {
            foreach (CCSPlayerController target in players)
            {
                targetname.Add(target.PlayerName);
            }
        }

        return ProcessTargetResultFlag.TargetFound;
    }

    /// <summary>
    /// Replies to a client with a given message describing a targetting failure reason.
    /// </summary>
    /// <param name="player">The player who executed the command.</param>
    /// <param name="resultFlag">The <see cref="ProcessTargetResultFlag"/> value indicating why it is failed.</param>
    public static void ReplyToTargetError(CCSPlayerController player, ProcessTargetResultFlag resultFlag)
    {
        switch (resultFlag)
        {
            case ProcessTargetResultFlag.TargetNone:
                player.PrintToChat(player.Localizer("No matching client"));
                break;
            case ProcessTargetResultFlag.TargetEmptyFilter:
                player.PrintToChat(player.Localizer("No matching clients"));
                break;
            case ProcessTargetResultFlag.TargetNotAlive:
                player.PrintToChat(player.Localizer("Target must be alive"));
                break;
            case ProcessTargetResultFlag.TargetNotDead:
                player.PrintToChat(player.Localizer("Target must be dead"));
                break;
            case ProcessTargetResultFlag.TargetImmune:
                player.PrintToChat(player.Localizer("Unable to target"));
                break;
            case ProcessTargetResultFlag.TargetNotHuman:
                player.PrintToChat(player.Localizer("Cannot target bot"));
                break;
            case ProcessTargetResultFlag.TargetAmbiguous:
                player.PrintToChat(player.Localizer("More than one client matched"));
                break;
        }
    }

    /// <summary>
    /// Retrieves a localized text string for the specified player and text key.
    /// </summary>
    /// <param name="player"> The player for whom the text should be localized.</param>
    /// <param name="text">The localization key to resolve.</param>
    /// <param name="args">Optional format arguments to replace placeholders (e.g. {0}, {1}) in the localized string.</param>
    /// <returns>
    /// The localized string formatted with the provided arguments, or the key itself 
    /// if no localized entry is found.
    /// </returns>
    public static string GetLocalizedText(CCSPlayerController player, string text, params object[] args)
    {
        return player.Localizer(text, args);
    }
}