using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Objects;

public class BonesManager : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private GameObject displayedGameObject;

    private Bone bone;

    /// <summary>
    /// Génère le segment du bones
    /// </summary>
    public void CreateSkeletonOnButton()
    {
       // bone = new Bone(mesh);
        //DisplayResult();
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
        sphereQL.transform.position = bone.qL;
        sphereQK.transform.position = bone.qK;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Instantiate(displayedGameObject, mesh.vertices[i], Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (bone != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(bone.qL, bone.qK);
        }
    }
}