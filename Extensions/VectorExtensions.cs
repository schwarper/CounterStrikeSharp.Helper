using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="Vector"/> class.
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// Creates a new <see cref="Vector"/> with the same components as the original.
    /// </summary>
    public static Vector Clone(this Vector vector)
    {
        return new Vector(vector.X, vector.Y, vector.Z);
    }

    /// <summary>
    /// Returns the Euclidean distance between two vectors.
    /// </summary>
    public static float Distance(this Vector vector, Vector other)
    {
        return MathF.Sqrt(vector.DistanceSquared(other));
    }

    /// <summary>
    /// Returns the squared distance between two vectors. 
    /// This avoids the cost of a square root operation and is faster for comparisons.
    /// </summary>
    public static float DistanceSquared(this Vector vector, Vector other)
    {
        return (vector.X - other.X) * (vector.X - other.X)
             + (vector.Y - other.Y) * (vector.Y - other.Y)
             + (vector.Z - other.Z) * (vector.Z - other.Z);
    }

    /// <summary>
    /// Checks whether all components of the vector are zero.
    /// </summary>
    public static bool IsZero(this Vector vector)
    {
        return vector.LengthSqr() == 0;
    }

    /// <summary>
    /// Returns a normalized version of the vector (length = 1).
    /// If the vector is zero, a zero vector is returned.
    /// </summary>
    public static Vector Normalized(this Vector v)
    {
        float length = MathF.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        if (length == 0f)
            return new Vector(0, 0, 0);

        return new Vector(v.X / length, v.Y / length, v.Z / length);
    }

    /// <summary>
    /// Returns the squared magnitude of the vector.
    /// </summary>
    public static float LengthSquared(this Vector v)
    {
        return v.X * v.X + v.Y * v.Y + v.Z * v.Z;
    }

    /// <summary>
    /// Returns the dot product of two vectors.
    /// </summary>
    public static float Dot(this Vector v1, Vector v2)
    {
        return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
    }

    /// <summary>
    /// Returns the angle in degrees between two vectors.
    /// </summary>
    public static float AngleBetween(this Vector a, Vector b)
    {
        float dot = a.Dot(b) / (a.Length() * b.Length());
        return MathF.Acos(Math.Clamp(dot, -1f, 1f)) * (180f / MathF.PI);
    }

    /// <summary>
    /// Returns a normalized version of a vector. 
    /// This is equivalent to <see cref="Normalized"/>, provided for completeness.
    /// </summary>
    public static Vector Normalize(this Vector vector)
    {
        float length = vector.Length();
        return length == 0f ? Vector.Zero : vector / length;
    }
}