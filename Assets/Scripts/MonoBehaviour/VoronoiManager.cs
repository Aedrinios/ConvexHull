using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json.Serialization;
using Objects;
using Statics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class VoronoiManager : MonoBehaviour
{
    public DelaunayManager delaunayManager;
    public Transform barryPrefab;
    public LineRenderer lrFlipPrefab;

    private List<Edge> edges = new List<Edge>();
    private List<Edge> colinearEdges = new List<Edge>();
    private List<Triangle> triangles = new List<Triangle>();
    private List<List<Point>> cells = new List<List<Point>>();
    
    
    private void Awake()
    {
        if (!TryGetComponent(out delaunayManager))
        {
        }
    }
    
    private void InitializeVoronoi()
    {
        Debug.Log("Voronoi : init");
        triangles = delaunayManager.triangles;
        ExtractEdgesFromTriangles();
        Debug.Log("Voronoi : triangles size : " + triangles.Count);
        Debug.Log("Voronoi : edges size : " + edges.Count);
    }

    public void VoronoiStart()
    {
        InitializeVoronoi();
        ComputeVoronoi();
        
    }

    private void ComputeVoronoi()
    {
        GetEdgesColinear();
        GetCells();
        //DebugColinearEdges();
        ShowVoronoi();
    }

    private void ShowVoronoi()
    {
        foreach (var cell in cells)
        {
            LineRenderer lr = Instantiate(lrFlipPrefab,Vector3.zero, Quaternion.identity);
            lr.positionCount = cell.Count;
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            for(int i = 0; i < cell.Count; i++)
            {
                lr.SetPosition(i, cell[i].Position);
            }
        }
    }

    private void GetCells()
    {
        foreach (var point in delaunayManager.sortedPoints)
        {
            List<Edge> cellEdges = new List<Edge>();
            for (int i = 0; i < edges.Count; i++)
            {
                Edge e = edges[i];
                if (point.Position == e.firstPoint.Position || point.Position == e.secondPoint.Position)//si l'edge est relié à mon sommet
                {
                    cellEdges.Add(colinearEdges[i]);
                }
            }

            List<Point> cellPointsUnsorted = GetPointsOfCell(cellEdges);
            List<Point> polygon = new List<Point>();
            cells.Add(GrahamScanStatic.Compute(cellPointsUnsorted));
        }
    }

    private List<Point> GetPointsOfCell(List<Edge> cellEdges)
    {
        List<Point> points = new List<Point>();

        foreach (var edge in cellEdges)
        {
            if(!points.Contains(edge.firstPoint)) points.Add(edge.firstPoint);
            if(!points.Contains(edge.secondPoint)) points.Add(edge.secondPoint);
        }

        return points;
    }

    private void DebugColinearEdges()
    {
        foreach (var edge in colinearEdges)
        {
            Debug.Log("Voronoi : itteration debug edge");
            LineRenderer lr = Instantiate(lrFlipPrefab,Vector3.zero, Quaternion.identity);
            lr.positionCount = 2;
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.SetPosition(0,edge.firstPoint.Position);
            lr.SetPosition(1,edge.secondPoint.Position);
        }
    }

    private void GetEdgesColinear()
    {
        Debug.Log("Voronoi : init");
        int T1;
        int T2;
        foreach(var edge in edges)
        {
            Debug.Log("Voronoi : itteration edge");
            (T1, T2) = edge.BelongsToTriangles(triangles);
            if (T1 >= 0 && T2 >= 0)//deux triangles
            {
                triangles[T1].CreateCircumcircle();
                triangles[T2].CreateCircumcircle();
                colinearEdges.Add(new Edge(new Point(triangles[T1].Center),new Point(triangles[T2].Center)));
            }
            else if(T1 >= 0 || T2 >= 0)
            {
                int T = T1 >= 0 ? T1 : T2;
                triangles[T].CreateCircumcircle();
                Point middleOfEdge = new Point((edge.firstPoint.Position + edge.secondPoint.Position) / 2f);
                colinearEdges.Add(new Edge(middleOfEdge,new Point(triangles[T].Center)));
            }
            else
            {
                Debug.LogError("Error : An Edge doesn't have a triangle.");
            }
        }
    }

    private void ExtractEdgesFromTriangles()
    {
        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.Edges)
            {
                if (!edges.Contains(edge)) edges.Add(edge);
            }
        }
    }
    
}