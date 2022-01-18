using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Objects;

public class BonesManager : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private GameObject displayedGameObject;

    private List<Point> points = new List<Point>();
    private Matrix3x3 covarianceMatrix = new Matrix3x3();
    private Point barycenter = new Point();
    private Vector3[] projectedPoints;
    private Vector3 qL = new Vector3();
    private Vector3 qK = new Vector3();
    private int powerSearch = 100;

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Creer la liste de points
    /// </summary>
    void Init()
    {
        foreach (Vector3 vertice in mesh.vertices)
            points.Add(new Point(vertice));
        projectedPoints = new Vector3[points.Count];
    }

    /// <summary>
    /// Génère le segment du bones
    /// </summary>
    public void CreateSkeletonOnButton()
    {
        (qL, qK) = Bones.CreateSkeleton(points, powerSearch);
        Debug.Log("QL : " + qL + " QK : " + qK);
        DisplayResult();
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