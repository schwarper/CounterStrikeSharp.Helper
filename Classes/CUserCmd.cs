using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.Helper.Classes;

/// <summary>
/// Provides a class for the <c>ProcessUsercmds</c> dynamic hook. 
/// </summary>
/// <param name="pointer">
/// Pointer to the native <c>CUserCmd</c> structure.
/// </param>
/// <remarks>
/// Example usage:
/// <code>
/// CUserCmd userCmd = new(hook.GetParam&lt;IntPtr&gt;(1));
/// QAngle? angle = userCmd.GetViewAngles();
/// if (angle is not null)
/// {
///     Console.WriteLine($@"Player view angles: {angle}");
/// }
/// </code>
/// </remarks>
public class CUserCmd(nint pointer)
{
    /// <summary>
    /// Gets the player's current view angles.
    /// </summary>
    /// <returns>
    /// A <see cref="QAngle"/> representing the player's view angles, or <c>null</c>
    /// if the underlying pointer or any required offsets are invalid.
    /// </returns>
    public unsafe QAngle? GetViewAngles()
    {
        if (pointer == 0)
            return null;

        nint baseCmd = Unsafe.Read<IntPtr>((void*)(pointer + 0x40));
        if (baseCmd == 0)
            return null;

        nint msgQAngle = Unsafe.Read<IntPtr>((void*)(baseCmd + 0x40));
        if (msgQAngle == 0)
            return null;

        QAngle viewAngles = new(msgQAngle + 0x18);
        return viewAngles.Handle != IntPtr.Zero ? viewAngles : null;
    }
}
