using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class CloudPointsManager : MonoBehaviour
{
    
    [SerializeField] GameObject pointGo;
    [SerializeField] Transform container;
    
    private LineRenderer convLr;
    
    private Point[] points;
    private Point barycenter = new Point();
    public Matrix4x4 covarianceMatrix;
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
    
    public void CalculateBarycenter()
    {
        foreach (Point p in points)
        {
            barycenter.Position += p.Position;
        }

        barycenter.Position /= points.Length;
    }

    public void CenteringPointToBarycenter()
    {
        foreach (Point p in points)
        {
            p.Position -= barycenter.Position;
        }
    }

    public float CalculateCovariance(int index1,int index2)
    {
        float covariance = 0f;
        foreach (Point p in points)
            covariance += (1 / points.Length) * (p.Position[index1] - barycenter.Position[index1]) 
                                              * (p.Position[index2] - barycenter.Position[index2]);

        return covariance;
    }

    public void CreateCovarianceMatrix()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
                covarianceMatrix[i,j] = CalculateCovariance(i, j);
        }
    }

    public float GetGreaterAbsoluteValueInVector3(Vector3 v)
    {
        return Mathf.Max(Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y)), Mathf.Abs(v.z));
    }
    
    // Eigen Vector c'est le nom anglais de Vecteur propre ptdr
    public void PowerIteration(Matrix4x4 matrix)
    {
        Vector3[] v = new Vector3[100];
        v[0] = new Vector3(1, 0, 0);
        Vector3 resultMatrix;
        float lambdaK;
        for (int k = 0; k < 100; k++)
        {
            resultMatrix = matrix * v[k];
            lambdaK = GetGreaterAbsoluteValueInVector3(resultMatrix);
            v[k + 1] = 1 / lambdaK * resultMatrix;
        }
    }
}
