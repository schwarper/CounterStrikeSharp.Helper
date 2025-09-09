using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CBasePlayerWeapon"/> class.
/// </summary>
public static class CBasePlayerWeaponExtensions
{
    /// <summary>
    /// Gets the weapon's designer name (e.g., "weapon_ak47").
    /// </summary>
    /// <param name="weapon">The <see cref="CBasePlayerWeapon"/> instance.</param>
    /// <returns>The designer name of the weapon as a string, or <c>null</c> if it cannot be retrieved.</returns>
    public static string? GetWeaponName(this CBasePlayerWeapon weapon)
    {
        return weapon.GetVData<CCSWeaponBaseVData>()?.Name;
    }

    /// <summary>
    /// Sets the ammo for a weapon's primary clip and/or reserve.
    /// </summary>
    /// <param name="weapon">The <see cref="CBasePlayerWeapon"/> instance to modify.</param>
    /// <param name="clip">The amount of ammo for the primary clip. Use -1 to ignore.</param>
    /// <param name="reserve">The amount of ammo for the reserve. Use -1 to ignore.</param>
    public static void SetWeaponAmmo(this CBasePlayerWeapon weapon, int clip = -1, int reserve = -1)
    {
        if (weapon.As<CCSWeaponBase>().GetVData<CCSWeaponBaseVData>() is not { } vdata)
            return;

        if (clip > -1)
        {
            weapon.Clip1 = clip;
            vdata.MaxClip1 = clip;
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_iClip1");
        }

        if (reserve > -1)
        {
            weapon.ReserveAmmo[0] = reserve;
            vdata.PrimaryReserveAmmoMax = reserve;
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_pReserveAmmo");
        }
    }

    /// <summary>
    /// Changes the visibility of the weapon entity.
    /// </summary>
    /// <param name="weapon">The <see cref="CBasePlayerWeapon"/> instance.</param>
    /// <param name="visible">Set to <c>true</c> to make the weapon visible (opaque), or <c>false</c> to make it invisible (transparent).</param>
    /// <remarks>Credits to LeviN for the original implementation.</remarks>
    public static void SetVisible(this CBasePlayerWeapon weapon, bool visible)
    {
        Color color = Color.FromArgb(
            visible ? 255 : 0,
            weapon.Render.R,
            weapon.Render.G,
            weapon.Render.B
        );

        weapon.RenderMode = RenderMode_t.kRenderTransAlpha;
        weapon.RenderFX = RenderFx_t.kRenderFxNone;
        weapon.Render = color;

        Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_nRenderMode");
        Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_nRenderFX");
        Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_clrRender");
    }

    /// <summary>
    /// Gets the owner of a weapon entity
    /// </summary>
    /// <param name="weapon">The weapon entity.</param>
    /// <returns>The <see cref="CCSPlayerController"/> instance for the player, or <c>null</c> if it doesn't exist.</returns>
    /// <remarks>Credits to daffyy for the original implementation.</remarks>
    public static CCSPlayerController? GetOwner(this CBasePlayerWeapon weapon)
    {
        if (weapon.OwnerEntity.Value is null)
            return null;

        CBasePlayerPawn pawn = new(NativeAPI.GetEntityFromIndex((int)weapon.OwnerEntity.Index));
        return Utilities.GetPlayerFromIndex((int)pawn.Controller.Index);
    }
}