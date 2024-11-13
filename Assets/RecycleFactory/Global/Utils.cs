using System.Collections;
using UnityEngine;

namespace RecycleFactory
{
    public static class Utils
    {
        // Map the directions to indices
        public static readonly Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)  // Left
        };

        /// <summary>
        /// Rotates the given direction by one step (90 degrees).
        /// sign = 1 for clockwise, sign = -1 for counterclockwise.
        /// </summary>
        public static Vector2Int RotateOnce(Vector2Int currentDirection, int sign)
        {
            sign = Mathf.Clamp(sign, -1, 1);

            int currentIndex = System.Array.IndexOf(directions, currentDirection);

            if (currentIndex == -1)
            {
                Debug.LogError("Invalid direction!");
                return currentDirection;
            }

            int newIndex = (int)Mathf.Repeat(currentIndex + sign, 4);

            return directions[newIndex];
        }

        /// <summary>
        /// Rotates the given direction by a specified delta (90 degrees per step).
        /// Delta can be positive (clockwise) or negative (counterclockwise).
        /// </summary>
        public static Vector3Int RotateXZ(Vector3Int currentDirection, int delta)
        {
            if (delta < 0)
                delta += 4;
            for (int i = 0; i < 4 - delta; i++)
            {
                currentDirection = new Vector3Int(
                    -currentDirection.z,
                    0,
                    currentDirection.x
                );
            }

            return currentDirection;
        }

        /// <summary>
        /// Rotates the given direction by a specified delta (90 degrees per step).
        /// Delta can be positive (clockwise) or negative (counterclockwise).
        /// </summary>
        public static Vector2Int RotateXY(Vector2Int currentDirection, int delta)
        {
            if (delta < 0)
                delta += 4;
            for (int i = 0; i < 4 - delta; i++)
            {
                currentDirection = new Vector2Int(
                    -currentDirection.y,
                    currentDirection.x
                );
            }

            return currentDirection;
        }

        public static Vector3 ProjectTo3D(this Vector2 vec2, float height = 0)
        {
            return new Vector3(vec2.x, height, vec2.y);
        }

        public static Vector2 ProjectTo2D(this Vector3 vec3)
        {
            return new Vector2(vec3.x, vec3.z);
        }

        /// <summary>
        /// Replaces each member of vector with 0 if it is 0 and 1 otherwise
        /// </summary>
        public static Vector2 UnsignedMask(this Vector2 vec)
        {
            return new Vector2(vec.x == 0 ? 0 : 1, vec.y == 0 ? 0 : 1);
        }

        /// <summary>
        /// Replaces each member of vector with 0 if it is 0, with 1 if it is positive and with -1 if it is negative
        /// </summary>
        public static Vector2 SignedMask(this Vector2 vec)
        {
            return new Vector2(Hexath.Ternarsign(vec.x), Hexath.Ternarsign(vec.y));
        }

        /// <summary>
        /// Replaces each member of vector with 0 if it is 0 and 1 otherwise
        /// </summary>
        public static Vector3 UnsignedMask(this Vector3 vec)
        {
            return new Vector3(vec.x == 0 ? 0 : 1, vec.y == 0 ? 0 : 1, vec.z == 0 ? 0 : 1);
        }

        /// <summary>
        /// Replaces each member of vector with 0 if it is 0, with 1 if it is positive and with -1 if it is negative
        /// </summary>
        public static Vector3 SignedMask(this Vector3 vec)
        {
            return new Vector3(Hexath.Ternarsign(vec.x), Hexath.Ternarsign(vec.y), Hexath.Ternarsign(vec.z));
        }
    }
}

// used for init-only setters
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}