using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class DelaunayManager : MonoBehaviour
{
    public LineRenderer lrPrefab;
    private CloudPointsManager cpm;
    private Point[] sortedPoints;
    private List<Edge> edges;
    private List<Triangle> triangles;

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
        int numberOfPointsPassed = 0;
        for (int i = 1; i < sortedPoints.Length; i++)
        {
            if (Math.Abs(sortedPoints[i].Position.x - initialX) < 0.001)
            {
                numberOfPointsPassed++;
            }
            else
            {
                break;
            }
        }

        //2b - Avec le point Pk+1, on construit la triangulation initiale Tk+1 dont le bord est le polygone triangulaire POk+1 = [P1P2...Pk+1]
        if (numberOfPointsPassed >= 2)
        {
            Point kplus1 = sortedPoints[numberOfPointsPassed + 1];
            for (int i = 0; i < numberOfPointsPassed; i++)
            {
                Triangulate(sortedPoints[i], sortedPoints[i+1], kplus1);
            }
            numberOfPointsPassed++;
        }
        
        //3 - A l'étape q, k+1<=q<n, on ajoute le point Pq+1 à la triangulation Tq afin d'obtenir la triangulation Tq+1
        // Le point Pq+1 n'appartenant pas au polygone POq, on va rechercher les arêtes de POq vues par Pq+1
        // puis construire les nouveaux triangles à partir de celles-ci et du Point Pq+1
        //3a - On recherche les arêtes de Pq "vues" depuis Pq+1 : une arête A est vue de Pq+1 si et seulement si 
        //le point Pq+1 appartient à int(E_), E_ étant le demi-espace délimité par A et ne contenant pas Pq+1
        //Avec un produit scalaire, on relie les points si l'angle est aigu
        for (int i = numberOfPointsPassed; i < sortedPoints.Length; i++)
        {
            
        }
    }

    private void Triangulate(Point a, Point b, Point c)
    {
        Edge edge1 = new Edge(a, b);
        Edge edge2 = new Edge(b, c);
        Edge edge3 = new Edge(c, a);
        edges.Add(edge1);
        edges.Add(edge2);
        edges.Add(edge3);
        triangles.Add(new Triangle(edge1, edge2, edge3));
    }
}