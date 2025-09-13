using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CCSPlayerPawn"/> class.
/// </summary>
public static class CCSPlayerPawnExtensions
{
    private const float SmokeRadius = 180.0f;

    /// <summary>
    /// Gets the player's eye position.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <returns>A <see cref="Vector"/> representing the eye position, or <c>null</c> if the pawn's origin is invalid.</returns>
    public static Vector? GetEyePosition(this CCSPlayerPawn playerPawn)
    {
        return playerPawn.AbsOrigin is not { } absOrigin
            ? null
            : new Vector(absOrigin.X, absOrigin.Y, absOrigin.Z + playerPawn.ViewOffset.Z);
    }

    /// <summary>
    /// Checks if the player is currently inside a smoke.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <returns><c>true</c> if the player is in smoke; otherwise, <c>false</c>.</returns>
    /// <remarks>Credits to Gold KingZ for the original implementation.</remarks>
    public static bool IsInSmoke(this CCSPlayerPawn playerPawn)
    {
        if (playerPawn.AbsOrigin is not { } absOrigin)
            return false;

        IEnumerable<CSmokeGrenadeProjectile> smokeProjectiles = Utilities.FindAllEntitiesByDesignerName<CSmokeGrenadeProjectile>("smokegrenade_projectile");
        foreach (CSmokeGrenadeProjectile smoke in smokeProjectiles)
        {
            if (smoke.DidSmokeEffect && (absOrigin - smoke.SmokeDetonationPos).Length() <= SmokeRadius)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Finds a weapon in the player's inventory by its designer name.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="designerName">The designer name of the weapon to find (e.g., "weapon_ak47").</param>
    /// <returns>The <see cref="CBasePlayerWeapon"/> instance if found; otherwise, <c>null</c>.</returns>
    public static CBasePlayerWeapon? GetWeaponByName(this CCSPlayerPawn playerPawn, string designerName)
    {
        return playerPawn.WeaponServices?.MyWeapons
            .Select(w => w.Value)
            .FirstOrDefault(w => w?.DesignerName == designerName);
    }

    /// <summary>
    /// Checks if the player has a weapon in the specified equipment slot.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="slot">The equipment slot (<see cref="gear_slot_t"/>) to check.</param>
    /// <returns><c>true</c> if the player has a weapon in the given slot; otherwise, <c>false</c>.</returns>
    /// <remarks>Credits to K4ryuu for the original implementation.</remarks>
    public static bool HasWeaponInSlot(this CCSPlayerPawn playerPawn, gear_slot_t slot)
    {
        return playerPawn.WeaponServices?.MyWeapons
            .Select(w => w.Value?.As<CCSWeaponBase>())
            .Any(w => w?.VData?.GearSlot == slot) ?? false;
    }

    /// <summary>
    /// Sets the scale of the player's model.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="scale">The scale factor to apply (e.g., 1.0 is normal size, 2.0 is double size).</param>
    public static void SetModelScale(this CCSPlayerPawn playerPawn, float scale)
    {
        if (playerPawn.CBodyComponent?.SceneNode is { } sceneNode)
        {
            sceneNode.Scale = scale;
            Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
        }
    }

    /// <summary>
    /// Changes the player's movement type (e.g., walk, noclip, fly).
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="moveType">The <see cref="MoveType_t"/> to apply.</param>
    public static void SetMoveType(this CCSPlayerPawn playerPawn, MoveType_t moveType)
    {
        playerPawn.MoveType = moveType;
        Schema.GetRef<MoveType_t>(playerPawn.Handle, "CBaseEntity", "m_nActualMoveType") = moveType;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_MoveType");
    }

    /// <summary>
    /// Freezes the player, preventing all movement.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    public static void Freeze(this CCSPlayerPawn playerPawn)
    {
        playerPawn.SetMoveType(MoveType_t.MOVETYPE_OBSOLETE);
    }

    /// <summary>
    /// Unfreezes the player, restoring normal movement.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    public static void Unfreeze(this CCSPlayerPawn playerPawn)
    {
        playerPawn.SetMoveType(MoveType_t.MOVETYPE_WALK);
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
    /// Gets if player is stuck or not.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <returns>
    /// <c>true</c> if the pawn is flagged as stuck by the movement services; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>Credits to xstage for the original implementation.</remarks>
    public static bool IsStuck(this CCSPlayerPawn playerPawn)
    {
        return playerPawn.MovementServices?.As<CCSPlayer_MovementServices>() is not { } movementServices
            ? false
            : !movementServices.InStuckTest && movementServices.StuckLast > 0;
    }


    /// <summary>
    /// Pushes the player back with a bounce effect.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <remarks>Credits to AquaVadis for the original implementation.</remarks>
    public static void Bounce(this CCSPlayerPawn playerPawn)
    {
        Vector vel = new(playerPawn.AbsVelocity.X, playerPawn.AbsVelocity.Y, playerPawn.AbsVelocity.Z);
        double speed = Math.Sqrt((vel.X * vel.X) + (vel.Y * vel.Y));

        vel *= -350 / (float)speed;
        vel.Z = vel.Z <= 0 ? 150 : Math.Min(vel.Z, 150);
        playerPawn.Teleport(playerPawn.AbsOrigin, playerPawn.EyeAngles, vel);
    }

    /// <summary>
    /// Dashes the player forward based on their view angle.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="power">The forward power of the dash.</param>
    /// <param name="verticalBoost">Additional vertical velocity to apply.</param>
    /// <remarks>Credits to AquaVadis for the original implementation.</remarks>
    public static void Dash(this CCSPlayerPawn playerPawn, int power, int verticalBoost)
    {
        if (playerPawn.AbsOrigin is not { } absOrigin)
            return;

        Vector _forward = new();
        Vector _origin = new(absOrigin.X, absOrigin.Y, absOrigin.Z);
        Vector _viewangles = new(0, playerPawn.EyeAngles.Y, playerPawn.EyeAngles.Z);

        _origin.X += _forward.X * 100;
        _origin.Y += _forward.Y * 100;
        NativeAPI.AngleVectors(_viewangles.Handle, _forward.Handle, 0, 0);

        Vector vVector = _forward * power;

        playerPawn.Teleport(null, null, new Vector3(vVector.X, vVector.Y, vVector.Z + verticalBoost));
    }

    /// <summary>
    /// Sets the player's health.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="health">The amount of health to set.</param>
    /// <param name="allowOverflow">If true, the player's max health will be increased to match the new health value if it exceeds the current max.</param>
    public static void SetHealth(this CCSPlayerPawn playerPawn, int health, bool allowOverflow = true)
    {
        if (allowOverflow && health > playerPawn.MaxHealth)
        {
            playerPawn.SetMaxHealth(health);
        }

        playerPawn.Health = health;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_iHealth");
    }

    /// <summary>
    /// Sets the player's maximum health.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="maxHealth">The maximum health value to set.</param>
    public static void SetMaxHealth(this CCSPlayerPawn playerPawn, int maxHealth)
    {
        playerPawn.MaxHealth = maxHealth;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_iMaxHealth");
    }

    /// <summary>
    /// Sets the player's armor and optionally provides a helmet or heavy armor.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="armor">The amount of armor to set.</param>
    /// <param name="helmet">If true, provides the player with a helmet.</param>
    public static void SetArmor(this CCSPlayerPawn playerPawn, int armor, bool helmet = false)
    {
        playerPawn.ArmorValue = armor;
        Utilities.SetStateChanged(playerPawn, "CCSPlayerPawnBase", "m_ArmorValue");

        if (helmet && playerPawn.ItemServices?.Handle is { } handle)
        {
            new CCSPlayer_ItemServices(handle).HasHelmet = true;
        }
    }

    /// <summary>
    /// Sets the player's movement speed modifier.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="modifier">The speed modifier to apply (1.0 is normal speed).</param>
    public static void SetSpeed(this CCSPlayerPawn playerPawn, float modifier)
    {
        playerPawn.VelocityModifier = modifier;
    }

    /// <summary>
    /// Gets the player's current velocity modifier.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <returns>The current speed modifier as a float.</returns>
    public static float GetSpeed(this CCSPlayerPawn playerPawn)
    {
        return playerPawn.VelocityModifier;
    }

    /// <summary>
    /// Determines whether the specified player is looking at the target player within a given angular threshold.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="targetPawn">The target pawn instance.</param>
    /// <param name="angleThreshold">
    /// The maximum allowed angle (in degrees) between the player's view direction and the direction to the target.
    /// Defaults to 50 degrees.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the target player lies within the viewing cone defined by 
    /// <paramref name="angleThreshold"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsLookingAtPlayer(this CCSPlayerPawn playerPawn, CCSPlayerPawn targetPawn, float angleThreshold = 50f)
    {
        if (playerPawn.AbsOrigin is not { } playerPawnAbsOrigin || targetPawn.AbsOrigin is not { } targetPawnAbsOrigin)
            return false;

        Vector forward = playerPawn.EyeAngles.AngleToForward();
        Vector directionToTarget = (targetPawnAbsOrigin - playerPawnAbsOrigin).Normalize();

        float dot = forward.Dot(directionToTarget);
        float angleBetween = MathF.Acos(dot) * (180f / MathF.PI);

        return angleBetween <= angleThreshold;
    }

    /// <summary>
    /// Sets the player's gravity scale.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="scale">The gravity scale to apply (1.0 is normal gravity).</param>
    public static void SetGravity(this CCSPlayerPawn playerPawn, float scale)
    {
        playerPawn.GravityScale = scale;
    }

    /// <summary>
    /// Finds a weapon by its designer name and forces the player to drop it.
    /// </summary>
    /// <param name="playerPawn">The <see cref="CCSPlayerPawn"/> instance.</param>
    /// <param name="designername">The designer name of the weapon to drop (e.g., "weapon_ak47").</param>
    /// <remarks>This method works by temporarily making the target weapon the active weapon before dropping it.</remarks>
    public static void DropWeaponByDesignername(this CCSPlayerPawn playerPawn, string designername)
    {
        if (playerPawn.WeaponServices is not { } weaponServices)
            return;

        CHandle<CBasePlayerWeapon>? weaponRaw = weaponServices.MyWeapons
            .FirstOrDefault(h => h.Value?.DesignerName == designername);

        if (weaponRaw == null)
            return;

        weaponServices.ActiveWeapon.Raw = weaponRaw;

        if (weaponServices.ActiveWeapon.Value is null)
            return;

        playerPawn.ItemServices?.As<CCSPlayer_ItemServices>().DropActivePlayerWeapon(weaponServices.ActiveWeapon.Value);
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
    /// Removes weapons from the player's inventory based on their equipment slot.
    /// </summary>
    /// <param name="playerPawn">The player pawn instance.</param>
    /// <param name="slot">A <see cref="gear_slot_t"/> indicating which slot to clear.</param>
    public static void RemoveWeaponBySlot(this CCSPlayerPawn playerPawn, gear_slot_t slot)
    {
        if (playerPawn.WeaponServices?.MyWeapons is not { } myWeapons || myWeapons.Count == 0)
            return;

        HashSet<gear_slot_t> existedSlots = [.. myWeapons
            .Select(w => w.Value?.As<CCSWeaponBase>().VData?.GearSlot)
            .OfType<gear_slot_t>()];

        if (existedSlots.Count == 0)
            return;

        if (existedSlots.Count == 1 && existedSlots.Single() == slot)
        {
            playerPawn.ItemServices?.As<CCSPlayer_ItemServices>().RemoveWeapons();
            return;
        }

        List<CBasePlayerWeapon?> toRemove = [.. myWeapons
            .Select(w => w.Value)
            .Where(w => w?.IsValid is true &&
                         w.As<CCSWeaponBase>().VData?.GearSlot is gear_slot_t sl &&
                         sl == slot)];

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