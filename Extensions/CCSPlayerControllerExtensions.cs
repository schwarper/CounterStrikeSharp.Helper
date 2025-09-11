using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CCSPlayerController"/> class.
/// </summary>
public static class CCSPlayerControllerExtensions
{
    /// <summary>
    /// Defines the types of screen fade effects.
    /// </summary>
    public enum ScreenFadeFlags
    {
        /// <summary>
        /// Fades from the specified color to the normal screen.
        /// </summary>
        FadeIn,
        /// <summary>
        /// Fades from the normal screen to the specified color.
        /// </summary>
        FadeOut,
        /// <summary>
        /// Fades to the specified color and remains faded without fading back.
        /// </summary>
        StayOut
    }

    /// <summary>
    /// Gets the player's associated pawn. This is a convenient shortcut for <c>player.PlayerPawn.Value</c>.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <returns>The <see cref="CCSPlayerPawn"/> instance for the player, or <c>null</c> if it doesn't exist or is invalid.</returns>
    public static CCSPlayerPawn? Pawn(this CCSPlayerController player)
    {
        return player.PlayerPawn.Value;
    }

    /// <summary>
    /// Sets the player's in-game money.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="money">The amount of money to set. Must be a non-negative value.</param>
    public static void SetMoney(this CCSPlayerController player, int money)
    {
        if (player.InGameMoneyServices is not { } moneyServices)
            return;

        moneyServices.Account = money < 0 ? 0 : money;
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_pInGameMoneyServices");
    }

    /// <summary>
    /// Sets the player's kill count on the scoreboard for the current round.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="kills">The number of kills to set.</param>
    /// <remarks>
    /// Credits to B3none for the original implementation idea.
    /// </remarks>
    public static void SetKills(this CCSPlayerController player, int kills)
    {
        if (player.ActionTrackingServices == null)
            return;

        player.Score = kills;
        player.ActionTrackingServices.NumRoundKills = kills;
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_iScore");
        Utilities.SetStateChanged(player, "CCSPlayerController_ActionTrackingServices", "m_iNumRoundKills");
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_pActionTrackingServices");
    }

    /// <summary>
    /// Changes the player's name.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="name">The new name to set for the player.</param>
    public static void SetName(this CCSPlayerController player, string name)
    {
        if (string.IsNullOrEmpty(name) || player.PlayerName == name)
            return;

        player.PlayerName = name;
        Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
    }

    /// <summary>
    /// Applies a colored fade effect to the player's screen.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="color">The color of the fade effect.</param>
    /// <param name="duration">The duration in seconds for the fade-in/fade-out transition.</param>
    /// <param name="holdTime">The duration in seconds the screen should hold the solid color.</param>
    /// <param name="flags">The type of fade effect to apply (e.g., FadeIn, FadeOut).</param>
    /// <param name="withPurge">If <c>true</c>, purges any existing fade effects before applying the new one.</param>
    /// <remarks>Credits to soilin for the original implementation.</remarks>
    public static void FadeScreen(this CCSPlayerController player, Color color, float duration, float holdTime, ScreenFadeFlags flags, bool withPurge = true)
    {
        const int ScreenFadeUserMessageId = 106;
        const float ScreenFadeFactor = 512.0f;

        UserMessage fadeMsg = UserMessage.FromId(ScreenFadeUserMessageId);

        int flag = flags switch
        {
            ScreenFadeFlags.FadeIn => 0x0001,
            ScreenFadeFlags.FadeOut => 0x0002,
            ScreenFadeFlags.StayOut => 0x0008,
            _ => 0x0001
        };

        if (withPurge)
            flag |= 0x0010;

        fadeMsg.SetInt("duration", (int)(duration * ScreenFadeFactor));
        fadeMsg.SetInt("hold_time", (int)(holdTime * ScreenFadeFactor));
        fadeMsg.SetInt("flags", flag);
        fadeMsg.SetInt("color", color.R | (color.G << 8) | (color.B << 16) | (color.A << 24));
        fadeMsg.Send(player);
    }

    /// <summary>
    /// Sets the player's clan tag, which is displayed on the scoreboard.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="tag">The clan tag string to display. Use <c>null</c> or an empty string to clear it.</param>
    public static void SetScoreTag(this CCSPlayerController player, string tag)
    {
        if (player.Clan == tag)
            return;

        player.Clan = tag;
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
        new EventNextlevelChanged(false).FireEventToClient(player);
    }

    /// <summary>
    /// Sends a translated chat message to the player using the plugin's localization files.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="plugin">The plugin instance that holds the localization files.</param>
    /// <param name="messageKey">The localization key for the message.</param>
    /// <param name="args">Optional arguments to format into the localized string.</param>
    public static void PrintToChatLocalized(this CCSPlayerController player, BasePlugin plugin, string messageKey, params object[] args)
    {
        if (player.IsBot || !player.IsValid)
            return;

        player.PrintToChat(plugin.Localizer.ForPlayer(player, messageKey, args));
    }
}