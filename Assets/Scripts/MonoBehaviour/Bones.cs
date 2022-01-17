using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public class Bones : MonoBehaviour
{
    [SerializeField] Mesh mesh;
    public List<Point> points = new List<Point>();
    [SerializeField] private GameObject displayedGameObject;
    private Vector3[] projectedPoints;
    // Eigen Vector c'est le nom anglais de Vecteur propre
    private Vector3 eigenVector;
    private float eigenValue;
    private Vector3 qL;
    private Vector3 qK;
    private Point barycenter = new Point();
    public Matrix3x3 covarianceMatrix = new Matrix3x3();
    private int powerSearch = 100;
    private bool doSomething = false;

    
    private int pointIndex = 0;

    void Start()
    {
        CreateSkeleton();
    }

    void CreateSkeleton()
    {
        init();
        CalculateBarycenter();
        CenteringPointToBarycenter();
        CreateCovarianceMatrix();
        PowerIteration();
        PointProjection();
        FindAndCorrectExtremeProjection();
        DisplayResult();
    }

    /// <summary>
    /// Creer la liste de points
    /// </summary>
    void init()
    {
        foreach (Vector3 vertice in mesh.vertices)
            points.Add(new Point(vertice));
    }

    
    public void CalculateBarycenter()
    {
        foreach (Point p in points)
        {
            barycenter.Position += p.Position;
        }

        barycenter.Position /= points.Count;
    }

    /// <summary>
    /// Change les la position des points pour
    /// que le barycenter soit l'origine des points
    /// </summary>
    public void CenteringPointToBarycenter()
    {
        foreach (Point p in points)
        {
            p.Position -= barycenter.Position;
        }
    }

    /// <summary>
    /// Quantification de variation des variables par rapport a eux 
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    /// <returns></returns>
    public float CalculateCovariance(int index1, int index2)
    {
        float covariance = 0f;
        foreach (Point p in points)
        {
            covariance +=
                ((p.Position[index1] - barycenter.Position[index1]) *
                 (p.Position[index2] - barycenter.Position[index2])) / points.Count;
        }
        return covariance;
    } 
    
    public void CreateCovarianceMatrix()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
                covarianceMatrix[i, j] = CalculateCovariance(i, j);
        }
    }

    /// <summary>
    /// Renvois la composante absolue la plus grande du vecteur (sans la convertir en abs)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public float GetGreaterAbsoluteValueInVector3(Vector3 v)
    {
        return Mathf.Max(Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y)), Mathf.Abs(v.z));
    }
        
    /// <summary>
    /// Calcule le vecteur propre de la matrix de covariance
    /// (Donne l'orientation du mesh)
    /// </summary>
     public void PowerIteration()
    {
        Vector3 vk = new Vector3(1, 0, 0);
        Vector3 resultMatrix;
        float lambdaK = 0;
        for (int k = 0; k < powerSearch; k++)
        {
            resultMatrix = covarianceMatrix * vk;
            lambdaK = GetGreaterAbsoluteValueInVector3(resultMatrix);
            vk = 1 / lambdaK * resultMatrix;
        }
        // Eigen Vector c'est le nom anglais de Vecteur propre
        eigenVector = vk.normalized;
        eigenValue = lambdaK;
    }

    /// <summary>
    /// créer l'axe du mesh avec la bonne longueur
    /// </summary>
    public void PointProjection()
    {
        projectedPoints = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            projectedPoints[i] = Vector3.Scale(Vector3.Scale(points[i].Position, eigenVector), eigenVector);
        }
    }

    /// <summary>
    /// Calcule le segment du mesh
    /// </summary>
    public void FindAndCorrectExtremeProjection()
    {
        Vector3 farthestPositivePoint = Vector3.zero;
        Vector3 farthestNegativePoint = Vector3.zero;
        foreach (Vector3 p in projectedPoints)
        {
            float alpha = Vector3.Dot(p, eigenVector);
            if (alpha > 0)
            {
                if (p.sqrMagnitude > farthestPositivePoint.sqrMagnitude)
                {
                    farthestPositivePoint = p;
                }
            }
            else if (alpha < 0)
            {
                if (p.magnitude > farthestNegativePoint.magnitude)
                {
                    farthestNegativePoint = p;
                }
            }
        }
        qK = farthestPositivePoint + barycenter.Position;
        qL = farthestNegativePoint + barycenter.Position;
    }

    /// <summary>
    /// affiche le resultat
    /// </summary>
    void DisplayResult()
    {
        GameObject sphereQL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject sphereQK = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sphereQL.GetComponent<Renderer>().material.color = Color.green;
        sphereQK.GetComponent<Renderer>().material.color = Color.black;
        sphereQL.transform.position = qL;
        sphereQK.transform.position = qK;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Instantiate(displayedGameObject, mesh.vertices[i], Quaternion.identity, transform);
        }
        
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(qL, qK);
    }
}