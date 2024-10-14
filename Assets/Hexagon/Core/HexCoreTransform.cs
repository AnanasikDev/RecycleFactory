using System.Collections.Generic;
using UnityEngine;

public static class HexTransform
{
    /// <summary>
    /// Iterates over all transforms attached to this transform as direct children. For recursive search see GetChildrenRecursive
    /// </summary>
    public static List<Transform> GetChildren(this Transform transform)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        return children;
    }

    /// <summary>
    /// Recursively iterates over all transforms nested to this transform
    /// </summary>
    public static List<Transform> GetChildrenRecursive(this Transform transform)
    {
        List<Transform> children = new List<Transform>();

        void inner(Transform transform)
        {
            foreach (Transform child in transform)
            {
                children.Add(child);

                if (child.childCount > 0) inner(child);
            }
        }

        inner(transform);

        return children;
    }
}