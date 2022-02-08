using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Objects
{
    public class Triangle
    {
        private Edge[] edges = new Edge[3];
        private Vector3 centerCircle;
        public Vector3 Center => centerCircle;
        private float rCircle;
        public float Ray => rCircle;

        public Triangle()
        {
            edges[0] = new Edge();
            edges[1] = new Edge();
            edges[2] = new Edge();
        }

        public Triangle(Edge a, Edge b, Edge c)
        {
            edges[0] = a;
            edges[1] = b;
            edges[2] = c;
        }

        public Triangle(List<Edge> newPoints)
        {
            if (newPoints == null || newPoints.Count <= 2) return;
            edges[0] = newPoints[0];
            edges[1] = newPoints[1];
            edges[2] = newPoints[2];
        }


        public void DisplayTriangle(ref LineRenderer lr)
        {
            lr.SetPosition(0, edges[0].A.Position);
            lr.SetPosition(1, edges[1].A.Position);
            lr.SetPosition(2, edges[2].A.Position);
            lr.SetPosition(3, edges[0].A.Position);
        }

        public bool Contains(Edge edgeContained)
        {
            foreach (Edge edge in edges)
            {
                if (edge == edgeContained)
                {
                    return true;
                }
            }

            return false;
        }

        public Point[] GetVertex()
        {
            return new Point[3] { edges[0].A, edges[1].A, edges[2].A };
        }

        public Edge[] GetEdge()
        {
            return edges;
        }

        public void GetCircumcircleRay()
        {
            Vector3 A = edges[0].A.Position;
            Vector3 B = edges[0].B.Position;
            Vector3 C = edges[1].B.Position;

            rCircle = ((A - B).magnitude * (B - C).magnitude * (C - A).magnitude) /
                      (2 * Vector3.Cross((A - B), (B - C)).magnitude);

            float alpha = Mathf.Pow((B - C).magnitude, 2) * Vector3.Dot((A - B), A - C)
                          / (2 * Mathf.Pow(Vector3.Cross((A - B), (B - C)).magnitude, 2));
            float beta = Mathf.Pow((A - C).magnitude, 2) * Vector3.Dot((B - A), B - C)
                         / (2 * Mathf.Pow(Vector3.Cross((A - B), (B - C)).magnitude, 2));
            float gamma = Mathf.Pow((A - B).magnitude, 2) * Vector3.Dot((C - A), C - B)
                          / (2 * Mathf.Pow(Vector3.Cross((A - B), (B - C)).magnitude, 2));

            centerCircle = alpha * A + beta * B + gamma * C;
        }
    }
}