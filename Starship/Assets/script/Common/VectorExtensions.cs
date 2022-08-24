using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Direction(this Vector2 source, Vector2 target)
    {
        return target - source;
    }

    public static float Distance(this Vector2 source, Vector2 target)
    {
        return Vector2.Distance(source, target);
    }

    public static float SqrDistance(this Vector2 source, Vector2 target)
    {
        return Vector2.SqrMagnitude(source - target);
    }
}
