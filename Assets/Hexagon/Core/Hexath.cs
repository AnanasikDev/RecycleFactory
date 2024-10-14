using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class for static math operations
/// </summary>
public static class Hexath
{
    /// <summary>
    /// Snaps the given number to the nearest float number within the given step. Rounding for float-point numbers with adjustable accuracy given as the "step" argument.
    /// </summary>
    /// <param name="number">Number to be rounded</param>
    /// <param name="step">Accuracy of rounding, modulo of the maximum difference with the original "number"</param>
    public static float SnapNumberToStep(this float number, float step)
    {
        float remainder = number % step;

        if (Mathf.Abs(remainder) < step / 2f) return number - remainder;
        else return number - remainder + step * Mathf.Sign(number);
    }

    /// <summary>
    /// Returns a point on the circumference with the given "radius" at the given "angle" in degrees, starting at the point (radius, 0) as in math.
    /// </summary>
    public static Vector2 GetCirclePointDegrees(float radius, float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;

        return GetCirclePointRadians(radius, angleRad);
    }

    /// <summary>
    /// Returns a point on the circumference with the given "radius" at the given "angle" in radians, starting at the point (radius, 0) as in math.
    /// </summary>
    public static Vector2 GetCirclePointRadians(float radius, float angle)
    {
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        return new Vector2(x, y);
    }
    
    /// <summary>
    /// Returns random point on the circumference of the given "radius".
    /// </summary>
    public static Vector2 GetRandomRingPoint(float radius)
    {
        return GetCirclePointDegrees(radius, UnityEngine.Random.Range(0f, 360f));
    }

    /// <summary>
    /// Returns random point on or within the circumference of the given "radius".
    /// </summary>
    public static Vector2 GetRandomCirclePoint(float radius)
    {
        return GetCirclePointDegrees(Random.Range(0, radius), UnityEngine.Random.Range(0f, 360f));
    }

    /// <summary>
    /// Least common multiple of two numbers
    /// </summary>
    public static int LCM(int a, int b)
    {
        for (int i = a; i < b * a; i += a)
        {
            if (i % a == 0 && i % b == 0) return i;
        }

        return -1;
    }
    /// <summary>
    /// Greatest common divisor of two numbers
    /// </summary>
    public static int GCD(int a, int b)
    {
        int min = a > b ? b : a;

        for (int i = min; i >= 0; i--)
        {
            if (b % i == 0 && a % i == 0) return i;
        }

        return -1;
    }

    /// <summary>
    /// Least common multiple of a list of numbers
    /// </summary>
    public static int LCM(List<int> nums)
    {
        int max = nums.Max();
        int lcm = max;

        while (true)
        {
            bool divisible = true;

            for (int i = 0; i < nums.Count; i++)
            {
                if (lcm % nums[i] != 0)
                {
                    divisible = false;
                    break;
                }
            }

            if (divisible)
            {
                return lcm;
            }

            lcm += max;
        }
    }

    /// <summary>
    /// Returns -1 if value is negative, 0 if value is 0, 1 if value is positive
    /// </summary>
    public static int Ternarsign(float value)
    {
        return (value > 0 ? 1 : (value < 0 ? -1 : 0));
    }

    /// <summary>
    /// Holds the input "value" at "max" when it is larger than "min", otherwise starts decreasing starting from "max".
    /// </summary>
    public static float Ramp(float value, float min, float max)
    {
        if (value > min) return max;
        return (max - min) + value;
    }

    /// <summary>
    /// Returns value if it's greater than min threshold, min otherside
    /// </summary>
    public static float MinLimit(float value, float min)
    {
        return value > min ? value : min;
    }

    /// <summary>
    /// Returns value if it's less than max threshold, or max otherwise
    /// </summary>
    public static float MaxLimit(float value, float max)
    {
        return value < max ? value : max;
    }

    public static bool NearlyEquals(this float a, float b, double epsilon = 1E-5) =>
        (a - b) <= epsilon;
}
