using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Objects
{
    public class Triangle
    {
        private Edge[] edges = new Edge[3];

        public Edge[] Edges
        {
            get { return edges; }
            set { edges = value; }
        }

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
            lr.SetPosition(0, edges[0].firstPoint.Position);
            lr.SetPosition(1, edges[1].firstPoint.Position);
            lr.SetPosition(2, edges[2].firstPoint.Position);
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

        public List<Vector3> GetVertex()
        {
            return new List<Vector3>
            {
                new Vector3(edges[0].firstPoint.Position.x, edges[0].firstPoint.Position.y,
                    edges[0].firstPoint.Position.z),
                new Vector3(edges[1].firstPoint.Position.x, edges[1].firstPoint.Position.y,
                    edges[1].firstPoint.Position.z),
                new Vector3(edges[2].firstPoint.Position.x, edges[2].firstPoint.Position.y,
                    edges[2].firstPoint.Position.z)
            };
        }

        public Point GetLastVertex(Edge edge)
        {
            Vector3 a = new Vector3(edge.firstPoint.Position.x, edge.firstPoint.Position.y, edge.firstPoint.Position.z);
            Vector3 b = new Vector3(edge.secondPoint.Position.x, edge.secondPoint.Position.y,
                edge.secondPoint.Position.z);
            List<Vector3> tmp = GetVertex();
            tmp.Remove(a);
            tmp.Remove(b);
            return new Point(tmp[0]);
        }

        public Tuple<Edge, Edge> GetOtherEdges(Edge a)
        {
            Edge firstEdge = new Edge();
            Edge secondEdge = new Edge();
            foreach (Edge e in edges)
            {
                if ((e.firstPoint.Position == a.secondPoint.Position ||
                     e.secondPoint.Position == a.secondPoint.Position) && e != a)
                    firstEdge = e;


                if ((e.firstPoint.Position == a.firstPoint.Position ||
                     e.secondPoint.Position == a.firstPoint.Position) && e != a)
                    secondEdge = e;
            }

            if (firstEdge.firstPoint.Position == Vector3.zero || secondEdge.firstPoint.Position == Vector3.zero ||
                firstEdge.secondPoint.Position == Vector3.zero || secondEdge.secondPoint.Position == Vector3.zero)
            {
                Debug.Log("WOAH");
            }


            return new Tuple<Edge, Edge>(firstEdge, secondEdge);
        }

        public void CreateCircumcircle()
        {
            Vector3 A = edges[0].firstPoint.Position;
            Vector3 B = edges[0].secondPoint.Position;
            Vector3 C = edges[1].secondPoint.Position;

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

        public bool VerifyDelaunayCriteria(Point[] pointsTriangulation)
        {
            CreateCircumcircle();
            List<Vector3> trianglePoint = GetVertex();
            foreach (Point p in pointsTriangulation)
            {
                if ((p.Position - centerCircle).magnitude < rCircle && !trianglePoint.Contains(p.Position))
                {
                    return false;
                }
            }

            return true;
        }

        public void SetEdges(Edge a, Edge a1, Edge a2)
        {
            edges[0] = a;
            if (a.secondPoint.Position == a1.firstPoint.Position)
            {
                edges[1] = a1;
            }
            else if (a.secondPoint.Position == a1.secondPoint.Position)
            {
                edges[1] = a1.Reverse();
            }

            if (a.firstPoint.Position == a2.secondPoint.Position)
            {
                edges[2] = a2;
            }
            else if (a.firstPoint.Position == a2.firstPoint.Position)
            {
                edges[2] = a2.Reverse();
            }
        }
    }
}