using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Objects;
using Statics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DelaunayManager : MonoBehaviour
{
    public LineRenderer lrPrefab;
    private CloudPointsManager cpm;
    private Point[] sortedPoints;
    private List<Edge> edges = new List<Edge>();
    private List<Triangle> triangles = new List<Triangle>();

    private bool drawTriangle = false;

    private void Awake()
    {
        if (!TryGetComponent(out cpm))
        {
        }

        InitializeDelaunay();
    }

    private void InitializeDelaunay()
    {
    }

    public void DelaunayStart()
    {
        IncrementalTriangulation(cpm.GetPoints());
    }

    public void IncrementalTriangulation(Point[] points)
    {
        if (points == null) return;
        if (points.Length <= 0) return;

        //1 - Les points sont triés par abscisse croissante
        sortedPoints = new Point[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            sortedPoints[i] = points[i];
        }

        Array.Sort(sortedPoints);

        //2 - On initialise le processus en calculant une 2-triangulation initiale
        //2a -On construit une suite de k - 1 arêtes colinéaires [P1,P2],..., [Pk-1, Pk] avec les k(>=2) points alignés
        float initialX = sortedPoints[0].Position.x;
        List<Point> pointsPassed = new List<Point> { sortedPoints[0] };
        for (int i = 1; i < sortedPoints.Length; i++)
        {
            if (Math.Abs(sortedPoints[i].Position.x - initialX) < 0.001)
            {
                pointsPassed.Add(sortedPoints[i]);
            }
            else
            {
                break;
            }
        }

        //2b - Avec le point Pk+1, on construit la triangulation initiale Tk+1 dont le bord est le polygone triangulaire POk+1 = [P1P2...Pk+1]
        int k = pointsPassed.Count;
        if (k >= 2)
        {
            Point kplus1 = sortedPoints[k + 1];
            for (int i = 0; i < k; i++)
            {
                Triangulate(sortedPoints[i], sortedPoints[i + 1], kplus1);
            }

            pointsPassed.Add(kplus1);
        }
        else
        {
            Triangulate(sortedPoints[0], sortedPoints[1], sortedPoints[2]);
            pointsPassed = new List<Point>() { sortedPoints[0], sortedPoints[1], sortedPoints[2] };
        }

        //3 - A l'étape q, k+1<=q<n, on ajoute le point Pq+1 à la triangulation Tq afin d'obtenir la triangulation Tq+1
        // Le point Pq+1 n'appartenant pas au polygone POq, on va rechercher les arêtes de POq vues par Pq+1
        // puis construire les nouveaux triangles à partir de celles-ci et du Point Pq+1
        //3a - On recherche les arêtes de Pq "vues" depuis Pq+1 : une arête A est vue de Pq+1 si et seulement si 
        //le point Pq+1 appartient à int(E_), E_ étant le demi-espace délimité par A et ne contenant pas Pq+1
        //Avec un produit scalaire, on relie les points si l'angle est aigu
        Point pqplus1 = new Point();
        for (int i = pointsPassed.Count; i < sortedPoints.Length; i++)
        {
            pqplus1 = sortedPoints[i];
            List<Point> polygon = new List<Point>();
            polygon = GrahamScanStatic.Compute(pointsPassed);

            for (int p = 0; p < polygon.Count; p++)
            {
                int indexNext = p + 1 < polygon.Count ? p + 1 : 0;
                Vector3 segment = polygon[indexNext].Position - polygon[p].Position;
                Vector3 normal = Vector3.Cross(segment.normalized, -Vector3.forward);
                Vector3 vectorToPoint = pqplus1.Position - polygon[p].Position;
                float dotProduct = Vector3.Dot(normal.normalized, vectorToPoint);
                if (dotProduct > 0)
                {
                    pointsPassed.Add(pqplus1);
                    Triangulate(polygon[p], polygon[indexNext], pqplus1);
                }
            }
        }

        DisplayIncrementation();
        drawTriangle = true;
    }

    void OnDrawGizmos()
    {
        Debug.Log("Draw Please");
        if (triangles.Count > 0 && drawTriangle)
        {
            triangles[0].GetCircumcircleRay();
            Gizmos.DrawSphere(triangles[0].Center, triangles[0].Ray);
        }
    }

    private void FlippingEdges()
    {
        // List<Edge> edgesTriangulation = edges;
        // while (edgesTriangulation.Count > 0)
        // {
        //     Edge A = edgesTriangulation[0];
        //     edgesTriangulation.Remove(A);
        //     if()
        // }
    }


    private void DisplayIncrementation()
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            LineRenderer lr = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
            lr.positionCount = 4;
            triangles[i].DisplayTriangle(ref lr);
        }
    }

    private void Triangulate(Point a, Point b, Point c)
    {
        Edge edge1 = new Edge(a, b);
        Edge edge2 = new Edge(b, c);
        Edge edge3 = new Edge(c, a);
        if (!edges.Contains(edge1)) edges.Add(edge1);
        if (!edges.Contains(edge2)) edges.Add(edge2);
        if (!edges.Contains(edge3)) edges.Add(edge3);
        triangles.Add(new Triangle(edge1, edge2, edge3));
    }
}