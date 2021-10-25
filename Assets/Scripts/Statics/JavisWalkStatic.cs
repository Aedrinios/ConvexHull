using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using System.Linq;

public static class JavisWalkStatic
{
    public static Vector3[] Calculate(Point[] points)
    {
        List<Vector3> result = new List<Vector3>();
        Vector3 mostLeftPoint = FindMostLeftPoint(points).Position;
        Vector3 end = new Vector3();

        do
        {
            result.Add(mostLeftPoint);
            end = points[0].Position;

            for (int i = 1; i < points.Length; i++)
            {
                if ((end == mostLeftPoint) || (counterclockwise(mostLeftPoint, end, points[i].Position) < 0))
                    end = points[i].Position;
            }
            mostLeftPoint = end;

            if (end == result[0])
            {
                break;
            }
        } while ((end != result[0]));

        result.Add(result[0]);
        return result.ToArray();
    }

    private static Point FindMostLeftPoint(Point[] points)
    {
        return points.Where(p => p.Position.x == (points.Min(y => y.Position.x))).First();
    }

    private static float counterclockwise(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Sign((p2.x - p1.x) * (p3.y - p1.y) - (p3.x - p1.x) * (p2.y - p1.y));
    }
}
