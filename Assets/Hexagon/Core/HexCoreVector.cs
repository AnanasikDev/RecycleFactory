using UnityEngine;

/// <summary>
/// Class for general vector operations (multiplication, division, and absolute value) for all vector types.
/// </summary>
public static class HexVectorOps
{
    /// <summary>
    /// Multiplies two Vector3 instances element-wise and returns the result.
    /// </summary>
    public static Vector3 Multiply(this Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

    /// <summary>
    /// Multiplies two Vector3Int instances element-wise and returns the result.
    /// </summary>
    public static Vector3Int Multiply(this Vector3Int a, Vector3Int b) => new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);

    /// <summary>
    /// Multiplies two Vector2 instances element-wise and returns the result.
    /// </summary>
    public static Vector2 Multiply(this Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

    /// <summary>
    /// Multiplies two Vector2Int instances element-wise and returns the result.
    /// </summary>
    public static Vector2Int Multiply(this Vector2Int a, Vector2Int b) => new Vector2Int(a.x * b.x, a.y * b.y);

    /// <summary>
    /// Divides two Vector3 instances element-wise and returns the result.
    /// </summary>
    /// <remarks>Ensure the second vector does not have zero components to avoid division by zero.</remarks>
    public static Vector3 Divide(this Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

    /// <summary>
    /// Divides two Vector3Int instances element-wise and returns the result.
    /// </summary>
    /// <remarks>Ensure the second vector does not have zero components to avoid division by zero.</remarks>
    public static Vector3Int Divide(this Vector3Int a, Vector3Int b) => new Vector3Int(a.x / b.x, a.y / b.y, a.z / b.z);

    /// <summary>
    /// Divides two Vector2 instances element-wise and returns the result.
    /// </summary>
    /// <remarks>Ensure the second vector does not have zero components to avoid division by zero.</remarks>
    public static Vector2 Divide(this Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

    /// <summary>
    /// Divides two Vector2Int instances element-wise and returns the result.
    /// </summary>
    /// <remarks>Ensure the second vector does not have zero components to avoid division by zero.</remarks>
    public static Vector2Int Divide(this Vector2Int a, Vector2Int b) => new Vector2Int(a.x / b.x, a.y / b.y);

    /// <summary>
    /// Returns a new Vector3 with the absolute values of the components.
    /// </summary>
    public static Vector3 Abs(this Vector3 vector) => new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));

    /// <summary>
    /// Returns a new Vector3Int with the absolute values of the components.
    /// </summary>
    public static Vector3Int Abs(this Vector3Int vector) => new Vector3Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));

    /// <summary>
    /// Returns a new Vector2 with the absolute values of the components.
    /// </summary>
    public static Vector2 Abs(this Vector2 vector) => new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));

    /// <summary>
    /// Returns a new Vector2Int with the absolute values of the components.
    /// </summary>
    public static Vector2Int Abs(this Vector2Int vector) => new Vector2Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
}

/// <summary>
/// Class for axis operations for all types of vectors.
/// </summary>
public static class HexVectorAxisOps
{
    /// <summary>
    /// Returns a copy of the given Vector3 with the Z component set to 0.
    /// </summary>
    public static Vector3 NullZ(this Vector3 vector) => new Vector3(vector.x, vector.y, 0);

    /// <summary>
    /// Creates a new Vector3 with the specified X component, retaining the original Y and Z components.
    /// </summary>
    public static Vector3 WithX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);

    /// <summary>
    /// Creates a new Vector3 with the specified Y component, retaining the original X and Z components.
    /// </summary>
    public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);

    /// <summary>
    /// Creates a new Vector3 with the specified Z component, retaining the original X and Y components.
    /// </summary>
    public static Vector3 WithZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

    /// <summary>
    /// Sets the X component of the Vector3.
    /// </summary>
    public static void SetX(this ref Vector3 vector, float x) => vector.x = x;

    /// <summary>
    /// Sets the Y component of the Vector3.
    /// </summary>
    public static void SetY(this ref Vector3 vector, float y) => vector.y = y;

    /// <summary>
    /// Sets the Z component of the Vector3.
    /// </summary>
    public static void SetZ(this ref Vector3 vector, float z) => vector.z = z;

    /// <summary>
    /// Creates a new Vector3Int with the specified X component, retaining the original Y and Z components.
    /// </summary>
    public static Vector3Int WithX(this Vector3Int vector, int x) => new Vector3Int(x, vector.y, vector.z);

    /// <summary>
    /// Creates a new Vector3Int with the specified Y component, retaining the original X and Z components.
    /// </summary>
    public static Vector3Int WithY(this Vector3Int vector, int y) => new Vector3Int(vector.x, y, vector.z);

    /// <summary>
    /// Creates a new Vector3Int with the specified Z component, retaining the original X and Y components.
    /// </summary>
    public static Vector3Int WithZ(this Vector3Int vector, int z) => new Vector3Int(vector.x, vector.y, z);

    /// <summary>
    /// Sets the X component of the Vector3Int.
    /// </summary>
    public static void SetX(this ref Vector3Int vector, int x) => vector.x = x;

    /// <summary>
    /// Sets the Y component of the Vector3Int.
    /// </summary>
    public static void SetY(this ref Vector3Int vector, int y) => vector.y = y;

    /// <summary>
    /// Sets the Z component of the Vector3Int.
    /// </summary>
    public static void SetZ(this ref Vector3Int vector, int z) => vector.z = z;

    /// <summary>
    /// Creates a new Vector2 with the specified X component, retaining the original Y component.
    /// </summary>
    public static Vector2 WithX(this Vector2 vector, float x) => new Vector2(x, vector.y);

    /// <summary>
    /// Creates a new Vector2 with the specified Y component, retaining the original X component.
    /// </summary>
    public static Vector2 WithY(this Vector2 vector, float y) => new Vector2(vector.x, y);

    /// <summary>
    /// Sets the X component of the Vector2.
    /// </summary>
    public static void SetX(this ref Vector2 vector, float x) => vector.x = x;

    /// <summary>
    /// Sets the Y component of the Vector2.
    /// </summary>
    public static void SetY(this ref Vector2 vector, float y) => vector.y = y;

    /// <summary>
    /// Creates a new Vector2Int with the specified X component, retaining the original Y component.
    /// </summary>
    public static Vector2Int WithX(this Vector2Int vector, int x) => new Vector2Int(x, vector.y);

    /// <summary>
    /// Creates a new Vector2Int with the specified Y component, retaining the original X component.
    /// </summary>
    public static Vector2Int WithY(this Vector2Int vector, int y) => new Vector2Int(vector.x, y);

    /// <summary>
    /// Sets the X component of the Vector2Int.
    /// </summary>
    public static void SetX(this ref Vector2Int vector, int x) => vector.x = x;

    /// <summary>
    /// Sets the Y component of the Vector2Int.
    /// </summary>
    public static void SetY(this ref Vector2Int vector, int y) => vector.y = y;
}

/// <summary>
/// Class for random generation of vectors.
/// </summary>
public static class HexVectorRandomOps
{
    /// <summary>
    /// Generates a random Vector3 with values between -1 and 1.
    /// </summary>
    public static Vector3 Random3D()
        => new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

    /// <summary>
    /// Generates a random Vector2 with values between -1 and 1.
    /// </summary>
    public static Vector2 Random2D()
        => new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

}


/// <summary>
/// Class for utility operations involving vectors:
/// <list type="bullet">
/// <item> Conversion operations </item>
/// <item> Distance functions </item>
/// <item> Nearly equals functions </item>
/// </list>
/// </summary>
public static class HexVectorUtils
{
    /// <summary>
    /// Converts a Vector3 to a Color with the desired alpha. Default alpha is 1.0f.
    /// </summary>
    public static Color VectorToColor(this Vector3 vector, float a = 1.0f) => new Color(vector.x, vector.y, vector.z, a);

    /// <summary>
    /// Converts a Color to a Vector3 representation. The result Vector3 values are within 0.0 - 1.0 range.
    /// </summary>
    public static Vector3 ColorToVector(this Color color) => new Vector3(color.r, color.g, color.b);

    /// <summary>
    /// Converts a Vector3 to a Vector2 by ignoring the Z component.
    /// </summary>
    public static Vector2 ConvertTo2D(this Vector3 vector3) => new Vector2(vector3.x, vector3.y);

    /// <summary>
    /// Converts a Vector2 to a Vector3 with Z set to 0.
    /// </summary>
    public static Vector3 ConvertTo3D(this Vector2 vector2) => new Vector3(vector2.x, vector2.y, 0);

    /// <summary>
    /// Converts a Vector2Int to a Vector2.
    /// </summary>
    public static Vector2 ConvertTo2D(this Vector2Int vector2Int) => new Vector2(vector2Int.x, vector2Int.y);

    /// <summary>
    /// Converts a Vector3Int to a Vector3.
    /// </summary>
    public static Vector3 ConvertTo3D(this Vector3Int vector3Int) => new Vector3(vector3Int.x, vector3Int.y, vector3Int.z);

    /// <summary>
    /// Rounds the components of a Vector2 to the nearest integer values and returns a Vector2Int.
    /// </summary>
    /// <returns>A new Vector2Int with rounded integer values.</returns>
    /// <remarks>
    /// This method uses Mathf.RoundToInt.
    /// </remarks>
    public static Vector2Int RoundToInt(this Vector2 vector2)
    {
        return new Vector2Int(
            Mathf.RoundToInt(vector2.x),
            Mathf.RoundToInt(vector2.y)
        );
    }

    /// <summary>
    /// Rounds the components of a Vector3 to the nearest integer values and returns a Vector3Int.
    /// </summary>
    /// <returns>A new Vector3Int with rounded integer values.</returns>
    /// <remarks>
    /// This method uses Mathf.RoundToInt.
    /// </remarks>
    public static Vector3Int RoundToInt(this Vector3 vector3)
    {
        return new Vector3Int(
            Mathf.RoundToInt(vector3.x),
            Mathf.RoundToInt(vector3.y),
            Mathf.RoundToInt(vector3.z)
        );
    }

    /// <summary>
    /// Ceils the components of a Vector2 and returns a Vector2Int.
    /// </summary>
    /// <returns>A new Vector2Int with ceiled integer values.</returns>
    /// <remarks>
    /// This method uses Mathf.CeilToInt.
    /// </remarks>
    public static Vector2Int CeilToInt(this Vector2 vector2)
    {
        return new Vector2Int(
            Mathf.CeilToInt(vector2.x),
            Mathf.CeilToInt(vector2.y)
        );
    }

    /// <summary>
    /// Ceils the components of a Vector3 and returns a Vector3Int.
    /// </summary>
    /// <returns>A new Vector3Int with ceiled integer values.</returns>
    /// <remarks>
    /// This method uses Mathf.CeilToInt.
    /// </remarks>
    public static Vector3Int CeilToInt(this Vector3 vector3)
    {
        return new Vector3Int(
            Mathf.CeilToInt(vector3.x),
            Mathf.CeilToInt(vector3.y),
            Mathf.CeilToInt(vector3.z)
        );
    }

    /// <summary>
    /// Floors the components of a Vector2 and returns a Vector2Int.
    /// </summary>
    /// <returns>A new Vector2Int with floored integer values.</returns>
    /// <remarks>
    /// This method uses Mathf.FloorToInt.
    /// </remarks>
    public static Vector2Int FloorToInt(this Vector2 vector2)
    {
        return new Vector2Int(
            Mathf.FloorToInt(vector2.x),
            Mathf.FloorToInt(vector2.y)
        );
    }

    /// <summary>
    /// Floors the components of a Vector3 and returns a Vector3Int.
    /// </summary>
    /// <returns>A new Vector3Int with floored integer values.</returns>
    /// <remarks>
    /// This method uses Mathf.FloorToInt.
    /// </remarks>
    public static Vector3Int FloorToInt(this Vector3 vector3)
    {
        return new Vector3Int(
            Mathf.FloorToInt(vector3.x),
            Mathf.FloorToInt(vector3.y),
            Mathf.FloorToInt(vector3.z)
        );
    }

    /// <summary>
    /// Calculates the squared distance between two Vector2.
    /// </summary>
    public static float SqrDistance(this Vector2 a, Vector2 b) => (a - b).sqrMagnitude;

    /// <summary>
    /// Calculates the squared distance between two Vector2Int.
    /// </summary>
    public static float SqrDistance(this Vector2Int a, Vector2Int b) => (a - b).sqrMagnitude;

    /// <summary>
    /// Calculates the squared distance between two Vector3.
    /// </summary>
    public static float SqrDistance(this Vector3 a, Vector3 b) => (a - b).sqrMagnitude;

    /// <summary>
    /// Calculates the squared distance between two Vector3Int.
    /// </summary>
    public static float SqrDistance(this Vector3Int a, Vector3Int b) => (a - b).sqrMagnitude;

    /// <summary>
    /// Calculates the squared distance between two Vector3 instances in 2D space (in XY-plane).
    /// </summary>
    public static float SqrDistanceXY(this Vector3 a, Vector3 b) => (a - b).NullZ().sqrMagnitude;

    /// <summary>
    /// Calculates the distance between two Vector2.
    /// </summary>
    public static float Distance(this Vector2 a, Vector2 b) => Vector2.Distance(a, b);

    /// <summary>
    /// Calculates the distance between two Vector2Int.
    /// </summary>
    public static float Distance(this Vector2Int a, Vector2Int b) => Vector2Int.Distance(a, b);

    /// <summary>
    /// Calculates the distance between two Vector3.
    /// </summary>
    public static float Distance(this Vector3 a, Vector3 b) => Vector3.Distance(a, b);

    /// <summary>
    /// Calculates the distance between two Vector3Int.
    /// </summary>
    public static float Distance(this Vector3Int a, Vector3Int b) => Vector3Int.Distance(a, b);

    /// <summary>
    /// Calculates the distance between two Vector3 instances in 2D space (in XY-plane).
    /// </summary>
    public static float DistanceXY(this Vector3 a, Vector3 b) => (a - b).NullZ().magnitude;

    /// <summary>
    /// Checks if two Vector3 instances are nearly equal based on an inaccuracy tolerance.
    /// </summary>
    public static bool NearlyEquals(this Vector3 a, Vector3 b, double inaccuracy = 1.0E-5) => Vector3.SqrMagnitude(a - b) < inaccuracy;

    /// <summary>
    /// Checks if two Vector3 instances are nearly equal based on an inaccuracy tolerance.
    /// </summary>
    public static bool NearlyEquals(this Vector2 a, Vector2 b, double inaccuracy = 1.0E-5) => Vector2.SqrMagnitude(a - b) < inaccuracy;

    /// <summary>
    /// Rotates the given Vector2 by the given degree.
    /// </summary>
    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        float tx = vector.x;
        float ty = vector.y;
        vector.x = (cos * tx) - (sin * ty);
        vector.y = (sin * tx) + (cos * ty);
        return vector;
    }
}

public static class HexVectorMath
{
    /// <summary>
    /// Clamps the given Vector2 to 0.0 - 1.0 range
    /// </summary>
    public static Vector2 Clamp01(this Vector2 vector) =>
        new Vector2(Mathf.Clamp01(vector.x), Mathf.Clamp01(vector.y));

    /// <summary>
    /// Clamps the given Vector3 to 0.0 - 1.0 range
    /// </summary>
    public static Vector3 Clamp01(this Vector3 vector) =>
        new Vector3(Mathf.Clamp01(vector.x), Mathf.Clamp01(vector.y), Mathf.Clamp01(vector.z));

}
