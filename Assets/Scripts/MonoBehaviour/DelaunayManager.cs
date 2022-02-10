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
    public Transform incrementalContainer;
    public Transform flipContainer;

    public LineRenderer lrPrefab;
    private CloudPointsManager cpm;
    private Point[] sortedPoints;
    private List<Edge> edges = new List<Edge>();
    public List<Triangle> triangles = new List<Triangle>();
    public List<Triangle> flippedTriangle = new List<Triangle>();

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
        IncrementalTriangulation(GenerateByClick.points);
    }

    public void IncrementalTriangulation(List<Point> points)
    {
        if (points == null) return;
        if (points.Count <= 0) return;

        //1 - Les points sont triés par abscisse croissante
        sortedPoints = new Point[points.Count];
        for (int i = 0; i < points.Count; i++)
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

        DisplayIncrementation(triangles, Color.red, incrementalContainer);
    }

    void OnDrawGizmos()
    {
        if (triangles.Count > 0)
        {
            foreach (Triangle t in triangles)
            {
                t.CreateCircumcircle();
                Gizmos.DrawWireSphere(t.Center, t.Ray);
            }
        }
    }

    public void FlippingEdges()
    {
        int count = 0;
        flippedTriangle = triangles;
        List<Edge> Ac = edges;
        Edge A = new Edge();
        Edge A1 = new Edge();
        Edge A2 = new Edge();
        Edge A3 = new Edge();
        Edge A4 = new Edge();
        int T1 = 0;
        int T2 = 0;
        Point S1 = new Point();
        Point S2 = new Point();
        Point S3 = new Point();
        Point S4 = new Point();
        bool testTriangle = true;
        while (Ac.Count > 0 && count < 1000)
        {
            A = Ac[0];
            Ac.Remove(A);
            (T1, T2) = A.BelongsToTriangles(triangles);
            if (T1 >= 0 && T2 >= 0 && !triangles[T1].VerifyDelaunayCriteria(triangles[T2].GetVertex()))
            {
                S3 = triangles[T1].GetLastVertex(A);
                S4 = triangles[T2].GetLastVertex(A);

                (A4, A1) = triangles[T1].GetOtherEdges(A);
                (A3, A2) = triangles[T2].GetOtherEdges(A);

                A = new Edge(S3, S4);
                triangles[T1].SetEdges(A, A1, A2);
                triangles[T2].SetEdges(A, A3, A4);

                Ac.Add(A1);
                Ac.Add(A2);
                Ac.Add(A3);
                Ac.Add(A4);
            }
            count++;
        }

        DisplayIncrementation(triangles, Color.green, flipContainer);
    }


    private void DisplayIncrementation(List<Triangle> listTriangles, Color color, Transform container)
    {
        for (int i = 0; i < listTriangles.Count; i++)
        {
            LineRenderer lr = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity, container);
            lr.startColor = color;
            lr.endColor = color;
            lr.positionCount = 3;
            listTriangles[i].DisplayTriangle(ref lr);
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

    private void DebugTriangle(List<Triangle> trianglesToTest)
    {
        foreach (Triangle t in trianglesToTest)
        {
            foreach (Edge e in t.Edges)
            {
                Debug.Log("first edge pos : " + e.firstPoint.Position);
                Debug.Log("second edge pos : " + e.secondPoint.Position);
            }
        }
    }
}