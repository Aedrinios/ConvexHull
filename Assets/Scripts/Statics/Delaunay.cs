using System;
using Objects;

public static class Delaunay
{
    public static void IncrementalTriangulation(Point[] points)
    {
        Point[] sortedPoints = new Point[points.Length];
        Array.Sort(points);
    }
}