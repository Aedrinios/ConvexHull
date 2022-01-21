using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class Bone
    {
        private Mesh mesh { get; set; }
        private Transform worldTransform;
        public List<Point> points { get; private set; } = new List<Point>();
        private Matrix3x3 covarianceMatrix = new Matrix3x3();
        private Point barycenter = new Point();
        public Vector3 qL = new Vector3();
        public Vector3 qK = new Vector3();
        public int bonesJointed = 0;//nombre de bones reliés à notre bone 
        private int powerSearch;
        
        public Bone(Mesh m,Transform worldTransform, int powerSearch = 100)
        {
            this.worldTransform = worldTransform;
            mesh = m;
            this.powerSearch = powerSearch;
            Init();
        }
        public Bone(GameObject go, int powerSearch = 100)
        {
            MeshFilter m;
            if (go.TryGetComponent<MeshFilter>(out m))
            {
                mesh = m.sharedMesh;
                this.powerSearch = powerSearch;
                Init();
            }
            else
            {
                Debug.LogError(go.name+  " n'a pas de mesh");
            }
        }
        /// <summary>
        /// Creer la liste de points
        /// Génère le segment du bones
        /// </summary>
        public void Init()
        {
            // foreach (Vector3 vertice in mesh.vertices)
            //     points.Add(new Point(vertice + worldTransform.position));
            // Renderer rend;
            // if (worldTransform.TryGetComponent(out rend))
            // {
            //     Debug.Log(worldTransform.name);
            //     Vector3 center = rend.bounds.center;
            //     Quaternion newRot = new Quaternion();
            //     
            //     Vector3 eulerAng = worldTransform.localRotation.eulerAngles; 
            //     newRot.eulerAngles = new Vector3(eulerAng.x, eulerAng.y, eulerAng.z);
            //     for (int i = 0; i < points.Count; i++)
            //     {
            //         points[i].Position = newRot * (points[i].Position - center) + center;
            //     }
            // }
            //
            //Debug.Log("QL : " + qL + " QK : " + qK);
            foreach (Vector3 v in mesh.vertices)
                points.Add( new Point((v + worldTransform.position)));

            //Rotation
            Quaternion newRotation = new Quaternion();
            newRotation.eulerAngles = worldTransform.rotation.eulerAngles;
            for (int i=0; i<points.Count; i++)
            {
                points[i].Position = newRotation * (points[i].Position - worldTransform.position) + worldTransform.position;
            }
            (qL, qK) = BonesStatic.CreateSkeleton(points, powerSearch);
            //______ACP______
        }
    }
}