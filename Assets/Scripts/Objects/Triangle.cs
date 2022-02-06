using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Objects
{
    public class Triangle
    {
        private Point[] points = new Point[3];

        public Triangle()
        {
            points[0] = new Point();
            points[1] = new Point();
            points[2] = new Point();
        }

        public Triangle(Point a, Point b, Point c)
        {
            points[0] = a;
            points[1] = b;
            points[2] = c;
        }

        public Triangle(List<Point> newPoints)
        {
            if (newPoints == null || newPoints.Count <= 2) return;
            points[0] = newPoints[0];
            points[1] = newPoints[1];
            points[2] = newPoints[2];
        }


        public void DisplayTriangle(ref LineRenderer lr)
        {
            lr.SetPosition(0, points[0].Position);
            lr.SetPosition(1, points[1].Position);
            lr.SetPosition(2, points[2].Position);
            lr.SetPosition(3, points[0].Position);
        }
    }
}