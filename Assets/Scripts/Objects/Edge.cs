using System.Collections.Generic;
using JetBrains.Annotations;
using Objects;
using UnityEngine;

namespace Objects
{
    public class Edge
    {
        private Point[] points = new Point[2];

        public Edge()
        {
            points[0] = new Point();
            points[1] = new Point();
        }

        public Edge(Point a, Point b)
        {
            points[0] = a;
            points[1] = b;
        }

        public Edge(List<Point> newPoints)
        {
            if (newPoints == null || newPoints.Count < 2) return;
            points[0] = newPoints[0];
            points[1] = newPoints[1];
        }

        [CanBeNull]
        public Edge FindPointsDifference(Edge b)
        {
            Point p1 = new Point();
            Point p2 = new Point();
            bool hasCommonPoint = false;
            foreach (Point pA in points)
            {
                foreach (Point pB in b.points)
                {
                    if (pA.Position == pB.Position)
                    {
                        hasCommonPoint = true;
                    }
                    else
                    {
                        p1 = pA;
                        p2 = pB;
                    }
                }
            }
            return hasCommonPoint ? new Edge(p1, p2) : null;
        }

        public void DisplayEdge(ref LineRenderer lr)
        {
            lr.SetPosition(i, points[0].Position);
            lr.SetPosition(points.Count, points[1].Position);

        }
    }
}