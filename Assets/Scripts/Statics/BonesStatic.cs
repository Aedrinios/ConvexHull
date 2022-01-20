using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public static class BonesStatic
{
    public static (Vector3, Vector3) CreateSkeleton(List<Point> pP, int powerSearch)
    {
        List<Point> points = new List<Point>();
        //Initialisation des valeurs
        Point barycenter = new Point();
        Matrix3x3 covarianceMatrix = new Matrix3x3();

        // Calcul du barycentre
        foreach (Point p in pP)
        {
            points.Add(new Point(p.Position));
            barycenter.Position += p.Position;
        }

        barycenter.Position /= points.Count;
        
        // Calcul de la matrice de covariance
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                covarianceMatrix[i, j] = CalculateCovariance(points, barycenter, i, j);


        // Change les la position des points pour
        // que le barycenter soit l'origine des points
        foreach (Point p in points)
            p.Position -= barycenter.Position;
        
        // Calcule le vecteur propre de la matrix de covariance
        // (Donne l'orientation du mesh)
        Vector3 vk = new Vector3(0, 0,1);
        Vector3 resultMatrix;
        float lambdaK = 0;
        for (int k = 0; k < powerSearch; k++)
        {
            resultMatrix = covarianceMatrix * vk;
            lambdaK = GetGreaterAbsoluteValueInVector3(resultMatrix);
            vk = 1 / lambdaK * resultMatrix;
        }

        Vector3 eigenVector = vk.normalized;

        // Projection des points
        Vector3[] projectedPoints = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
            projectedPoints[i] = Vector3.Dot(points[i].Position, eigenVector) *eigenVector;
        

        // Calcul des extremes du segment 
        Vector3 qK = Vector3.zero;
        Vector3 qL = Vector3.zero;
        foreach (Vector3 p in projectedPoints)
        {
            float alpha = Vector3.Dot(p, eigenVector);
            if (alpha > 0)
            {
                if (p.sqrMagnitude > qK.sqrMagnitude)
                    qK = p;
            }
            else if (alpha < 0)
            {
                if (p.magnitude > qL.magnitude)
                    qL = p;
            }
        }
        return( qK + barycenter.Position, qL + barycenter.Position);
    }

    /// <summary>
    /// Quantification de variation des variables par rapport a eux 
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    /// <returns></returns>
    private static float CalculateCovariance(List<Point> points, Point barycenter, int index1, int index2)
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

    /// <summary>
    /// Renvois la composante absolue la plus grande du vecteur (sans la convertir en abs)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private static float GetGreaterAbsoluteValueInVector3(Vector3 v)
    {
        float xAbs = Mathf.Abs(v.x);
        float yAbs = Mathf.Abs(v.y);
        float zAbs = Mathf.Abs(v.z);

        if (xAbs < yAbs)
        {
            if (yAbs < zAbs) return v.z;

            return v.y;
        }
        if (xAbs < zAbs) return v.z;

        return v.x;
    }
}