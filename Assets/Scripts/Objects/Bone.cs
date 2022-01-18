using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class Bone
    {
        public Mesh mesh { get; private set; }

        private List<Point> points = new List<Point>();
        private Matrix3x3 covarianceMatrix = new Matrix3x3();
        private Point barycenter = new Point();
        private Vector3[] projectedPoints;
        public Vector3 qL { get; private set; } = new Vector3();
        public Vector3 qK { get; private set; }= new Vector3();
        private int powerSearch;
        
        public Bone(Mesh m, int powerSearch = 100)
        {
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
        void Init()
        {
            foreach (Vector3 vertice in mesh.vertices)
                points.Add(new Point(vertice));
            projectedPoints = new Vector3[points.Count];
            
            (qL, qK) = BonesStatic.CreateSkeleton(points, powerSearch);
            Debug.Log("QL : " + qL + " QK : " + qK);
        }
    }
}