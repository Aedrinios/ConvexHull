using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Objects;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CloudPointsManager : MonoBehaviour
{
    
    [SerializeField] GameObject pointGo;
    [SerializeField] Transform container;
    
    private LineRenderer convLr;
    
    private Point[] points;
    private Vector3[] projectedPoints;
    private Vector3 eigenVector;
    private float eigenValue;
    private Vector3 qL;
    private Vector3 qK;
    private Point barycenter = new Point();
    public Matrix4x4 covarianceMatrix;
    private int powerSearch = 100;
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
        points = CloudPointsStatic.Create2DCloudPoints(50);
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
