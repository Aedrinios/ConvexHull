using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public class CloudPointsManager : MonoBehaviour
{

    public int amountOfPoints;
    [SerializeField] GameObject pointGo;
    [SerializeField] Transform container;
    
    private LineRenderer convLr;
    
    private Point[] points;

    private bool doSomething = false;

    private int pointIndex = 0;
    
    private void Awake()
    {
        if (!TryGetComponent(out convLr))
        {
            throw new System.Exception("Wsh met un LineRenderer sur l'enfant de l'enfant stp");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            doSomething = true;
            if (points != null)
            {
                Delaunay.IncrementalTriangulation(points);
            }
            
            else
                throw new System.Exception("Pas assez de points mon reuf");
        }
    }

    public void GenerateCloudsPoints()
    {
        for (int i = 0; i < container.childCount; i++)
        {
            PointController pc;
            if (container.GetChild(i).TryGetComponent(out pc))
            {
                pc.DestroyGO();
            }
        }
        points = CloudPointsStatic.Create2DCloudPoints(amountOfPoints);
        for (int i = 0; i < points.Length; i++)
        {
            points[i].Go = Instantiate(pointGo, points[i].Position, Quaternion.identity, container);
        }
    }
    public void ResetLineRenderer()
    {
        convLr.positionCount = 0;
    }

    public Point[] GetPoints()
    {
        return points;
    }

    public LineRenderer GetLineRenderer()
    {
        return convLr;
    }
}
