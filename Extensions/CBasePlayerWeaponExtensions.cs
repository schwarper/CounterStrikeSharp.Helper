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
    /// Retrieves the designer name of the specified weapon from its weapon data.
    /// </summary>
    /// <param name="weapon">The weapon instance to get the name from.</param>
    /// <returns>The name of the weapon as a string, or <c>null</c> if the weapon data is not available.</returns>
    public static string? GetWeaponName(this CBasePlayerWeapon weapon)
    {
        return weapon.GetVData<CCSWeaponBaseVData>()?.Name;
    }

    /// <summary>
    /// Sets the primary clip and/or reserve ammunition for the specified weapon.
    /// </summary>
    /// <param name="weapon">The weapon instance to modify.</param>
    /// <param name="clip">The amount of ammunition to set in the primary clip. If set to -1, the current clip ammo will not be changed.</param>
    /// <param name="reserve">The amount of ammunition to set in the reserve. If set to -1, the current reserve ammo will not be changed.</param>
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
    /// Sets the visibility of a weapon entity.
    /// </summary>
    /// <param name="weapon">The weapon entity.</param>
    /// <param name="value">
    /// True to make the weapon visible (opaque),
    /// false to make it invisible (fully transparent).
    /// </param>
    /// <remarks>Credits to LeviN for the original implementation.</remarks>
    public static void SetVisible(this CBasePlayerWeapon weapon, bool value)
    {
        Color color = Color.FromArgb(
            value ? 255 : 0,
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
        CBasePlayerPawn pawn = new(NativeAPI.GetEntityFromIndex((int)weapon.OwnerEntity.Index));
        return Utilities.GetPlayerFromIndex((int)pawn.Controller.Index);
    }
}