using System;
using System.Collections;
using Objects;
using UnityEngine;

public static class Delaunay
{
    public static void IncrementalTriangulation(Point[] points)
    {
        Debug.Log("Youhou je suis appelé !");
        if (points == null) return;
        if (points.Length <= 0) return;
        
        Debug.Log("Debut delaunay !");
        Point[] sortedPoints = new Point[points.Length];
        Array.Sort(points);
        
    }
}