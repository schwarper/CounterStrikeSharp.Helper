using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CCSPlayerController"/> class.
/// </summary>
public static class CCSPlayerControllerExtensions
{
    /// <summary>
    /// Defines the types of screen fade effects.
    /// </summary>
    public enum FadeFlags
    {
        /// <summary>
        /// Fades from the specified color to the normal screen.
        /// </summary>
        FADE_IN,
        /// <summary>
        /// Fades from the normal screen to the specified color.
        /// </summary>
        FADE_OUT,
        /// <summary>
        /// Fades to the specified color and remains faded.
        /// </summary>
        FADE_STAYOUT
    }

    /// <summary>
    /// Gets the player's associated pawn. This is a convenient shortcut for <c>player.PlayerPawn.Value</c>.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <returns>The <see cref="CCSPlayerPawn"/> instance for the player, or <c>null</c> if it doesn't exist.</returns>
    public static CCSPlayerPawn? Pawn(this CCSPlayerController player)
    {
        return player.PlayerPawn.Value;
    }

    /// <summary>
    /// Gets the player's eye position.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <returns>A <see cref="Vector"/> representing the player's eye position, or <c>null</c> if the pawn is not valid.</returns>
    public static Vector? GetEyePosition(this CCSPlayerController player)
    {
        return player.Pawn()?.GetEyePosition();
    }

    /// <summary>
    /// Checks if the player is currently inside a smoke.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <returns><c>true</c> if the player is in smoke; otherwise, <c>false</c>.</returns>
    public static bool IsInSmoke(this CCSPlayerController player)
    {
        return player.Pawn()?.IsInSmoke() ?? false;
    }

    /// <summary>
    /// Finds and returns a weapon from the player's inventory by its designer name (e.g., "weapon_ak47").
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="designername">The designer name of the weapon to find.</param>
    /// <returns>The <see cref="CBasePlayerWeapon"/> instance if found; otherwise, <c>null</c>.</returns>
    public static CBasePlayerWeapon? GetWeaponByDesignername(this CCSPlayerController player, string designername)
    {
        return player.Pawn()?.GetWeaponByDesignername(designername);
    }

    /// <summary>
    /// Sets the scale of the player's model.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="value">The scale factor to apply to the model (e.g., 1.0 is normal size).</param>
    public static void SetModelSize(this CCSPlayerController player, float value)
    {
        player.Pawn()?.SetModelSize(value);
    }

    /// <summary>
    /// Changes the movement type of the player's pawn (e.g., noclip, fly).
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="moveType">The new <see cref="MoveType_t"/> to apply.</param>
    public static void ChangeMoveType(this CCSPlayerController player, MoveType_t moveType)
    {
        player.Pawn()?.ChangeMoveType(moveType);
    }

    /// <summary>
    /// Freezes the player in place, preventing any movement.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    public static void Freeze(this CCSPlayerController player)
    {
        player.Pawn()?.Freeze();
    }

    /// <summary>
    /// Unfreezes the player, restoring normal movement.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    public static void Unfreeze(this CCSPlayerController player)
    {
        player.Pawn()?.Unfreeze();
    }

    /// <summary>
    /// Buries the player's pawn, typically making it non-solid and placing it under the ground.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    public static void Bury(this CCSPlayerController player)
    {
        player.Pawn()?.Bury();
    }

    /// <summary>
    /// Unburies the player's pawn, restoring its normal state.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    public static void Unbury(this CCSPlayerController player)
    {
        player.Pawn()?.Unbury();
    }

    /// <summary>
    /// Gets the path of the model file for the player's pawn.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <returns>The model name as a string, or <c>null</c> if the pawn is not valid.</returns>
    public static string? GetModelName(this CCSPlayerController player)
    {
        return player.Pawn()?.GetModelName();
    }

    /// <summary>
    /// Removes a specific weapon from the player's inventory by its designer name.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="designername">The designer name of the weapon to remove.</param>
    public static void RemoveWeaponByDesignername(this CCSPlayerController player, string designername)
    {
        player.Pawn()?.RemoveWeaponByDesignername(designername);
    }

    /// <summary>
    /// Removes weapons from the player's inventory based on their equipment slots.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="slots">A <see cref="HashSet{T}"/> of <see cref="gear_slot_t"/> representing the slots to clear.</param>
    public static void RemoveWeaponBySlot(this CCSPlayerController player, HashSet<gear_slot_t> slots)
    {
        player.Pawn()?.RemoveWeaponBySlot(slots);
    }

    /// <summary>
    /// Sets the player's in-game money amount.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="money">The amount of money to set.</param>
    public static void SetMoney(this CCSPlayerController player, int money)
    {
        if (player.InGameMoneyServices is not { } moneyServices)
            return;

        moneyServices.Account = money;
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_pInGameMoneyServices");
    }

    /// <summary>
    /// Sets the number of kills for the player in the current round.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="kills">The number of kills to set.</param>
    /// <remarks>Credits to B3none for the original implementation.</remarks>
    public static void SetClientKills(this CCSPlayerController player, int kills)
    {
        if (player.ActionTrackingServices == null)
            return;

        player.ActionTrackingServices.NumRoundKills = kills;
        Utilities.SetStateChanged(player, "CCSPlayerController_ActionTrackingServices", "m_iNumRoundKills");
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_pActionTrackingServices");
    }

    /// <summary>
    /// Applies a colored fade effect to the player's screen.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="color">The color of the fade effect.</param>
    /// <param name="hold">The duration in seconds the screen should hold the solid color.</param>
    /// <param name="fade">The duration in seconds for the fade-in/fade-out transition.</param>
    /// <param name="flags">The type of fade effect to apply (e.g., FADE_IN, FADE_OUT).</param>
    /// <param name="withPurge">If <c>true</c>, purges any existing fade effects before applying the new one.</param>
    /// <remarks>Credits to soilin for the original implementation.</remarks>
    public static void ColorScreen(this CCSPlayerController player, Color color, float hold = 0.1f, float fade = 0.2f, FadeFlags flags = FadeFlags.FADE_IN, bool withPurge = true)
    {
        UserMessage fadeMsg = UserMessage.FromId(106);

        fadeMsg.SetInt("duration", Convert.ToInt32(fade * 512));
        fadeMsg.SetInt("hold_time", Convert.ToInt32(hold * 512));

        int flag = flags switch
        {
            FadeFlags.FADE_IN => 0x0001,
            FadeFlags.FADE_OUT => 0x0002,
            FadeFlags.FADE_STAYOUT => 0x0008,
            _ => 0x0001
        };

        if (withPurge)
            flag |= 0x0010;

        fadeMsg.SetInt("flags", flag);
        fadeMsg.SetInt("color", color.R | (color.G << 8) | (color.B << 16) | (color.A << 24));
        fadeMsg.Send(player);
    }

    /// <summary>
    /// Sets the player's clan tag, which is displayed on the scoreboard.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="tag">The clan tag string to display. Use <c>null</c> or an empty string to clear it.</param>
    public static void SetScoreTag(this CCSPlayerController player, string? tag)
    {
        if (string.IsNullOrEmpty(tag) || player.Clan == tag)
            return;

        player.Clan = tag;
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
        new EventNextlevelChanged(false).FireEventToClient(player);
    }

    /// <summary>
    /// Sends a chat message to the player, translated to their selected language.
    /// </summary>
    /// <param name="player">The <see cref="CCSPlayerController"/> instance.</param>
    /// <param name="instance">The plugin instance that holds the localization files.</param>
    /// <param name="messageKey">The localization key for the message.</param>
    /// <param name="args">Optional arguments to format into the localized string.</param>
    public static void SendLocalizedMessage(this CCSPlayerController player, BasePlugin instance, string messageKey, params object[] args)
    {
        if (player.IsBot)
            return;

        player.PrintToChat(instance.Localizer.ForPlayer(player, messageKey, args));
    }
}