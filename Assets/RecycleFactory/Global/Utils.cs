using UnityEngine;

namespace RecycleFactory
{
    public static class Utils
    {
        // Map the directions to indices
        private static readonly Vector2Int[] directions = new Vector2Int[]
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
        public static Vector2Int Rotate(Vector2Int currentDirection, int delta)
        {
            int currentIndex = System.Array.IndexOf(directions, currentDirection);

            if (currentIndex == -1)
            {
                Debug.LogError("Invalid direction!");
                return currentDirection;
            }

            int newIndex = (int)Mathf.Repeat(currentIndex + delta, 4);

            return directions[newIndex];
        }
    }
}