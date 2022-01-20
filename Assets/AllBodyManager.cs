using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public class AllBodyManager : MonoBehaviour
{
    // [SerializeField] private GameObject head,
    //     torso,
    //     armL1,
    //     armL2,
    //     armR1,
    //     armR2,
    //     legL1,
    //     legL2,
    //     legR1,
    //     legR2;
    // Start is called before the first frame update
    // void Start()
    // {
    //     Bone torsoBone = new Bone(torso);
    //     Bone armL1Bone = new Bone(armL1);
    //     Bone armL2Bone = new Bone(armL2);
    //     Bone armR1Bone = new Bone(armR1);
    //     Bone armR2Bone = new Bone(armR2);
    //     Bone legL1Bone = new Bone(legL1);
    //     Bone legL2Bone = new Bone(legL2);
    //     Bone legR1Bone = new Bone(legR1);
    //     Bone legR2Bone = new Bone(legR2);
    //     DisplayResult(torsoBone);
    //     DisplayResult(armL1Bone);
    //     DisplayResult(armL2Bone);
    //     DisplayResult(armR1Bone);
    //     DisplayResult(armR2Bone);
    //     DisplayResult(legL1Bone);
    //     DisplayResult(legL2Bone);
    //     DisplayResult(legR1Bone);
    //     DisplayResult(legR2Bone);
    //     
    // }
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject displayedGameObject;
    private List<Bone> bones = new List<Bone>();

    private void Start()
    {
        for (int i = 0; i < model.transform.childCount; i++)
        {
            Transform trans = model.transform.GetChild(i);
            MeshFilter mf;
            if (trans.TryGetComponent(out mf))
            {
                Bone b = new Bone(mf.sharedMesh, trans);
                bones.Add(b);
                DisplayResult(b);
            }
            else
            {
                Debug.LogError(trans.name + " n'a pas de MeshFilter");
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (bones != null)
        {
            foreach (Bone b in bones)
            {
                Gizmos.color = Color.yellow;
                // Gizmos.DrawLine(b.qL + b.TransformOrigin.position, b.qK+ b.TransformOrigin.position);
                Gizmos.DrawLine(b.qL, b.qK);
                Debug.Log( " ql :" +b.qL+ " qk :"+ b.qK);
            }
        }
    }
    void DisplayResult(Bone bone)
    {
        if (bone.points != null && bone.points.Count != 0)
        {
            GameObject go = new GameObject();
            
            // GameObject sphereQL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // GameObject sphereQK = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            // sphereQK.transform.localScale = Vector3.one * 0.001f; 
            // sphereQL.transform.localScale = Vector3.one * 0.001f; 
            // sphereQL.GetComponent<Renderer>().material.color = Color.green;
            // sphereQK.GetComponent<Renderer>().material.color = Color.black;
            // sphereQL.transform.position = bone.qL;
            // sphereQK.transform.position = bone.qK;

            for (int i = 0; i < bone.points.Count; i++)
            {
                Instantiate(displayedGameObject, bone.points[i].Position, Quaternion.identity, go.transform);
            }
        }
    }
}
