using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CCSPlayerPawn"/> class.
/// </summary>
public static class CCSPlayerPawnExtensions
{
    private const float _smokeRadius = 180.0f;

    /// <summary>
    /// Gets the player's eye position.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <returns>A <see cref="Vector"/> representing the eye position, or <c>null</c> if the pawn's origin is not valid.</returns>
    public static Vector? GetEyePosition(this CCSPlayerPawn playerPawn)
    {
        return playerPawn.AbsOrigin is not { } absOrigin
            ? null
            : new Vector(absOrigin.X, absOrigin.Y, absOrigin.Z + playerPawn.ViewOffset.Z);
    }

    /// <summary>
    /// Checks if the player is currently inside a smoke.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <returns><c>true</c> if the player is in smoke; otherwise, <c>false</c>.</returns>
    /// <remarks>Credits to Gold KingZ for the original implementation.</remarks>
    public static bool IsInSmoke(this CCSPlayerPawn playerPawn)
    {
        if (playerPawn.AbsOrigin is not { } absOrigin)
            return false;

        IEnumerable<CSmokeGrenadeProjectile> entities = Utilities.FindAllEntitiesByDesignerName<CSmokeGrenadeProjectile>("smokegrenade_projectile");
        foreach (CSmokeGrenadeProjectile entity in entities)
        {
            if (!entity.DidSmokeEffect)
                continue;

            float dist = (absOrigin - entity.SmokeDetonationPos).Length();
            if (dist <= _smokeRadius)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Finds a weapon in the player's inventory by its designer name (e.g., "weapon_ak47").
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="designername">The designer name of the weapon to find.</param>
    /// <returns>The <see cref="CBasePlayerWeapon"/> instance if found; otherwise, <c>null</c>.</returns>
    public static CBasePlayerWeapon? GetWeaponByDesignername(this CCSPlayerPawn playerPawn, string designername)
    {
        return playerPawn.WeaponServices?.MyWeapons
            .Select(w => w.Value)
            .FirstOrDefault(w => w?.DesignerName == designername);
    }

    /// <summary>
    /// Sets the scale of the player's model.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="value">The scale factor to apply to the model (e.g., 1.0 is normal size).</param>
    public static void SetModelSize(this CCSPlayerPawn playerPawn, float value)
    {
        playerPawn.CBodyComponent!.SceneNode!.Scale = value;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
    }

    /// <summary>
    /// Changes the player's movement type (e.g., walk, noclip, fly).
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="moveType">The <see cref="MoveType_t"/> to apply.</param>
    public static void ChangeMoveType(this CCSPlayerPawn playerPawn, MoveType_t moveType)
    {
        playerPawn.MoveType = moveType;
        Schema.GetRef<MoveType_t>(playerPawn.Handle, "CBaseEntity", "m_nActualMoveType") = moveType;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_MoveType");
    }

    /// <summary>
    /// Freezes the player pawn, preventing all movement.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    public static void Freeze(this CCSPlayerPawn playerPawn)
    {
        playerPawn.ChangeMoveType(MoveType_t.MOVETYPE_OBSOLETE);
    }

    /// <summary>
    /// Unfreezes the player, restoring normal movement.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    public static void Unfreeze(this CCSPlayerPawn playerPawn)
    {
        playerPawn.ChangeMoveType(MoveType_t.MOVETYPE_WALK);
    }

    /// <summary>
    /// Buries the player's pawn, typically making it non-solid and placing it under the ground.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    public static void Bury(this CCSPlayerPawn playerPawn)
    {
        if (playerPawn.AbsOrigin is not { } absOrigin)
            return;

        Vector position = new(absOrigin.X, absOrigin.Y, absOrigin.Z - 10.0f);
        playerPawn.Teleport(position, playerPawn.AbsRotation, playerPawn.AbsVelocity);
    }

    /// <summary>
    /// Unburies the player's pawn, restoring its normal state.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    public static void Unbury(this CCSPlayerPawn playerPawn)
    {
        if (playerPawn.AbsOrigin is not { } absOrigin)
            return;

        Vector position = new(absOrigin.X, absOrigin.Y, absOrigin.Z + 10.0f);
        playerPawn.Teleport(position, playerPawn.AbsRotation, playerPawn.AbsVelocity);
    }

    /// <summary>
    /// Gets the path of the model file for the player's pawn.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <returns>The model name as a string, or <c>null</c> if it cannot be retrieved.</returns>
    public static string? GetModelName(this CCSPlayerPawn playerPawn)
    {
        return playerPawn.CBodyComponent?.SceneNode?.GetSkeletonInstance().ModelState.ModelName;
    }

    /// <summary>
    /// Removes a weapon from the player's inventory by its designer name.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="designername">The designer name of the weapon to remove.</param>
    public static void RemoveWeaponByDesignername(this CCSPlayerPawn playerPawn, string designername)
    {
        if (playerPawn.WeaponServices?.MyWeapons is not { } myWeapons || myWeapons.Count == 0)
            return;

        List<CBasePlayerWeapon?> toRemove = [.. myWeapons
            .Select(w => w.Value)
            .Where(w => w?.IsValid is true && w.DesignerName == designername)];

        playerPawn.RemoveWeaponsInternal(toRemove);
    }

    /// <summary>
    /// Removes weapons from the player's inventory based on their equipment slots.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="slots">A <see cref="HashSet{T}"/> of <see cref="gear_slot_t"/> indicating which slots to clear.</param>
    public static void RemoveWeaponBySlot(this CCSPlayerPawn playerPawn, HashSet<gear_slot_t> slots)
    {
        if (slots.Count == 0 || playerPawn.WeaponServices?.MyWeapons is not { } myWeapons || myWeapons.Count == 0)
            return;

        HashSet<gear_slot_t> existedSlots = [.. myWeapons
            .Select(w => w.Value?.As<CCSWeaponBase>().VData?.GearSlot)
            .OfType<gear_slot_t>()];

        if (existedSlots.Count == 0)
            return;

        if (existedSlots.All(s => slots.Contains(s)))
        {
            playerPawn.ItemServices?.As<CCSPlayer_ItemServices>().RemoveWeapons();
            return;
        }

        List<CBasePlayerWeapon?> toRemove = [.. myWeapons
            .Select(w => w.Value)
            .Where(w => w?.IsValid is true &&
                         w.As<CCSWeaponBase>().VData?.GearSlot is gear_slot_t slot &&
                         slots.Contains(slot))];

        playerPawn.RemoveWeaponsInternal(toRemove);
    }

    private static void RemoveWeaponsInternal(this CCSPlayerPawn playerPawn, List<CBasePlayerWeapon?> weapons)
    {
        if (weapons.Count == 0 || playerPawn.WeaponServices?.ActiveWeapon.Value is not { } activeWeapon)
            return;

        bool removeActiveWeapon = false;
        foreach (CBasePlayerWeapon? weapon in weapons)
        {
            if (weapon == null)
                continue;

            if (weapon == activeWeapon)
            {
                removeActiveWeapon = true;
                continue;
            }

            weapon.AddEntityIOEvent("Kill", weapon);
        }

        if (removeActiveWeapon)
        {
            API.Modules.Timers.Timer timer = new(0.1f, () =>
            {
                if (activeWeapon?.IsValid is not true)
                    return;

                playerPawn.ItemServices?.As<CCSPlayer_ItemServices>().DropActivePlayerWeapon(activeWeapon);
                activeWeapon.AddEntityIOEvent("Kill", activeWeapon, delay: 0.1f);
            });
        }
    }
}