using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Objects
{
    public class Point
    {
        public Vector3 Position;
        public GameObject Go;

        public Point()
        {
            Position = Vector3.zero;
        }

        public Point(Vector3 pos)
        {
            this.Position = pos;
        }
        
        public Point(GameObject go, Vector3 pos)
        {
            this.Go = go;
            this.Position = pos;
        }
        public void ApplyMat(Material mat)
        {
            Renderer rend;
            if (Go.TryGetComponent(out rend))
            {
                rend.material = mat;
            }
        }
    }
}