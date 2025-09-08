using System.Drawing;
using System.Globalization;
using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="ConVarExtensions"/> class.
/// </summary>
public static class ConVarExtensions
{
    /// <summary>
    /// Gets the value of a convar as a string.
    /// </summary>
    /// <param name="cvar">The convar instance.</param>
    /// <returns>A string representation of the cvar's value.</returns>
    /// <remarks>
    /// This method handles various <see cref="ConVarType"/> values, formatting them appropriately. For example, a <see cref="ConVarType.Color"/> will be formatted as "R,G,B,A", and floating-point numbers will use a period as the decimal separator. Returns "Invalid" if the type is not supported or invalid.
    /// </remarks>
    public static string GetCvarStringValue(this ConVar cvar)
    {
        return cvar.Type switch
        {
            ConVarType.Bool => cvar.GetPrimitiveValue<bool>().ToString(),
            ConVarType.Int16 => cvar.GetPrimitiveValue<short>().ToString(),
            ConVarType.UInt16 => cvar.GetPrimitiveValue<ushort>().ToString(),
            ConVarType.Int32 => cvar.GetPrimitiveValue<int>().ToString(),
            ConVarType.UInt32 => cvar.GetPrimitiveValue<uint>().ToString(),
            ConVarType.Int64 => cvar.GetPrimitiveValue<long>().ToString(),
            ConVarType.UInt64 => cvar.GetPrimitiveValue<ulong>().ToString(),
            ConVarType.Float32 => cvar.GetPrimitiveValue<float>().ToString(CultureInfo.InvariantCulture),
            ConVarType.Float64 => cvar.GetPrimitiveValue<double>().ToString(CultureInfo.InvariantCulture),
            ConVarType.String => cvar.StringValue,
            ConVarType.Color => cvar.GetPrimitiveValue<Color>() is var col ? $"{col.R},{col.G},{col.B},{col.A}" : "Invalid",
            ConVarType.Vector2 => cvar.GetPrimitiveValue<Vector2>() is var v2 ? $"{v2.X},{v2.Y}" : "Invalid",
            ConVarType.Vector3 => cvar.GetPrimitiveValue<Vector3>() is var v3 ? $"{v3.X},{v3.Y},{v3.Z}" : "Invalid",
            ConVarType.Vector4 => cvar.GetPrimitiveValue<Vector4>() is var v4 ? $"{v4.X},{v4.Y},{v4.Z},{v4.W}" : "Invalid",
            ConVarType.Qangle => cvar.GetPrimitiveValue<QAngle>() is var q ? $"{q.X},{q.Y},{q.Z}" : "Invalid",
            ConVarType.Invalid => "Invalid",
            _ => "Invalid"
        };
    }
}
