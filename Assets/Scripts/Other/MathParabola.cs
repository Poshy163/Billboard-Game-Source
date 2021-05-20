using System;
using UnityEngine;
// ReSharper disable All
//Formatted
public class MathParabola
{
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> func = x => -4f * height * x * x + 4f * height * x;
        var vector = Vector3.Lerp(start, end, t);
        return new Vector3(vector.x, func(t) + Mathf.Lerp(start.y, end.y, t), vector.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> func = x => -4f * height * x * x + 4f * height * x;
        return new Vector2(Vector2.Lerp(start, end, t).x, func(t) + Mathf.Lerp(start.y, end.y, t));
    }
}