using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Objects;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
    [SerializeField] private float joinEpsilon = 5f;
    [SerializeField] private float epsilonBonesManquant = 15f;
    [Header("Debug")]
    [SerializeField] private bool showPoints = false;
    [SerializeField] private bool showJoints = false;
    [SerializeField] private GameObject displayedGameObject;
    [SerializeField] private Color boneColor = Color.yellow;
    [SerializeField] private Color jointBoneColor = Color.blue;
    [SerializeField] private Color jointColor = Color.red;
    [SerializeField] private float jointRadius = 1f;
    private List<Bone> bones = new List<Bone>();
    private List<Joint> joints = new List<Joint>();
    

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
                if(showPoints)DisplayResult(b);
            }
            else
            {
                Debug.LogError(trans.name + " n'a pas de MeshFilter");
            }
        }
        
        JointBones();
        ConvertToJoints();
        AddMissingBones();
        Debug.Log("Nb of bone ends : " + bones.Count*2);
        Debug.Log("Nb of joints found : " + joints.Count);
    }


    private void AddMissingBones()
    {
        for(int i = 0;i < joints.Count; i++)
        {
            Joint currentJoint = joints[i];
            for(int j = i;j < joints.Count; j++){
                Joint otherJoint = joints[j];
                Debug.Log("CurrentJoint : " + currentJoint.pos + " otherJoint : " + otherJoint.pos);
                if (!CheckProximity(currentJoint.pos, otherJoint.pos, 0.001f) && 
                CheckProximity(currentJoint.pos, otherJoint.pos, epsilonBonesManquant))
                {
                    Bone b = new Bone(currentJoint.pos,otherJoint.pos);
                    bones.Add(b);
                }
            }    
        }
    }

    private void ConvertToJoints()
    {
        joints = new List<Joint>();
        for (int i = 0; i < bones.Count; i++)
        {
            Bone b = bones[i];
            if (!CheckPointAlreadyStored(b.qL)) joints.Add(new Joint(b.qL));
            if (!CheckPointAlreadyStored(b.qK)) joints.Add(new Joint(b.qK));
        }
    }

    private bool CheckPointAlreadyStored(Vector3 p)
    {
        bool alreadyStored = false;
        foreach (Joint j in joints)
        {
            if ((j.pos - p).magnitude < 0.01f)
            {
                alreadyStored = true;
                break;
            }
        }

        return alreadyStored;
    }

    private void JointBones()
    {
        for(int i = 0;i < bones.Count; i++)
        {
            Bone currentBone = bones[i];
            
            for(int j = i;j < bones.Count; j++){
                Bone otherBone = bones[j];
                
                bool matched = false;//stocke si on a trouvé une correspondance
                if (JoinByProximity(ref currentBone.qL, ref otherBone.qL, joinEpsilon)) matched = true;
                if (JoinByProximity(ref currentBone.qL, ref otherBone.qK, joinEpsilon)) matched = true;
                if (JoinByProximity(ref currentBone.qK, ref otherBone.qL, joinEpsilon)) matched = true;
                if (JoinByProximity(ref currentBone.qK, ref otherBone.qK, joinEpsilon)) matched = true;
                currentBone.bonesJointed += matched||matched ? 1 : 0;
            }    
        }
    }

    public static bool JoinByProximity(ref Vector3 a, ref Vector3 b, float epsilon)
    {
        if (CheckProximity(a, b, epsilon))
        {
            Vector3 moy = (a + b) / 2.0f;
            a = moy;
            b = moy;
            return true;
        }
        return false;
    }

    private static bool CheckProximity(Vector3 a, Vector3 b, float epsilon)
    {
        float absX = Mathf.Abs(a.x - b.x);
        float absY = Mathf.Abs(a.y - b.y);
        float absZ = Mathf.Abs(a.z - b.z);
        float proximity = new Vector3(absX, absY, absZ).magnitude;
        return proximity < epsilon;
    }

    private void OnDrawGizmos()
    {
        if (bones != null)
        {
            if (showJoints)
            {
                foreach (Joint joint in joints)
                {
                    Gizmos.color = jointColor;
                    Gizmos.DrawSphere(joint.pos, jointRadius);
                }
            }
            else
            {
                foreach (Bone b in bones)
                {
                    Gizmos.color = b.isJoin ? jointBoneColor : boneColor;
                    // Gizmos.DrawLine(b.qL + b.TransformOrigin.position, b.qK+ b.TransformOrigin.position);
                    Gizmos.DrawLine(b.qL, b.qK);
                    Gizmos.color = jointColor;
                    Gizmos.DrawSphere(b.qL, jointRadius);
                    Gizmos.DrawSphere(b.qK, jointRadius);
                    //Debug.Log( " ql :" +b.qL+ " qk :"+ b.qK);
                }
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
