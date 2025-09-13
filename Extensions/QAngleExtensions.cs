using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="QAngle"/> class.
/// </summary>
public static class QAngleExtensions
{
    /// <summary>
    /// Creates a new <see cref="QAngle"/> with the same components as the original.
    /// </summary>
    public static QAngle Clone(this QAngle angles)
    {
        return new QAngle
        {
            X = angles.X,
            Y = angles.Y,
            Z = angles.Z
        };
    }

    /// <summary>
    /// Converts the specified <see cref="QAngle"/> to a forward-facing direction vector.
    /// </summary>
    public static Vector AngleToForward(this QAngle angles)
    {
        float pitch = MathF.PI / 180f * angles.X;
        float yaw = MathF.PI / 180f * angles.Y;

        float x = MathF.Cos(pitch) * MathF.Cos(yaw);
        float y = MathF.Cos(pitch) * MathF.Sin(yaw);
        float z = -MathF.Sin(pitch);

        return new Vector(x, y, z).Normalize();
    }
}
