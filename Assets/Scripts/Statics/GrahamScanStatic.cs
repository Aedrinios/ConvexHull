using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

namespace Statics
{
    public static class GrahamScanStatic
    {
        public static List<Point> SortPoints(List<Point> points, Point bc = null)
        {
            if (bc == null )
            {
                throw new Exception("IMPLEMENTE LE BARRYCENTRE EN FAITE");
            }
        
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Position -= bc.Position;
            }

            points = points.OrderBy(x => 
                    ComparePoints(bc.Position, x.Position, bc.Position + Vector3.right * 2)).ToList();
            
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Position += bc.Position;
            }
            return points;
        }
        
        /// <summary>
        ///  returns true in the cases where the first angle is bigger than the second angle.
        /// In the cases where the angles are equal, or the first distance is less
        /// than the second distance
        /// </summary>
        /// <param name="bc"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool ComparePoints(Vector3 bc, Vector3 p1, Vector3 p2)
        {
            float angle1 = GetAngle(bc, p1);
            float angle2 = GetAngle(bc, p2);
            if (angle1 < angle2)
                return true;
            float dist1 = GetDistance(bc, p1);
            float dist2 = GetDistance(bc, p2);
            if (angle1 == angle2 && dist1 < dist2)
                return true;
            return false;
        }
        
        /// <summary>
        /// return angle from 0 to 2PI for p1 relative to p2
        /// </summary>
        /// <param name="side1">p1</param>
        /// <param name="side2">p2</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 side1, Vector3 side2)
        {
            Vector3 v =side2-side1;
            float angle = Mathf.Atan2(v.x, v.y);
            if (angle <= 0)
                angle = 2 * Mathf.PI + angle;
            return angle;
        }
        /// <summary>
        /// Calculate between three points 
        /// </summary>
        /// <param name="p0">Pivot</param>
        /// <param name="p1">p-1</param>
        /// <param name="p2">p+1</param>
        /// <returns>Radians</returns>
        public static float GetAngle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 side1 = p1-p0;
            Vector3 side2 = p2-p0;
            // float angle =  Vector3.Angle(side1, side2);
            float angle = GetAngle(side1, side2);
            return angle;
            // float numerator = p2.y * (p1.x - p3.x) + p1.y * (p3.x - p2.x) + p3.y * (p2.x - p1.x);
            // float denominator = (p2.x - p1.x) * (p1.x - p3.x) + (p2.y - p1.y) * (p1.y - p3.y);
            // float ratio = numerator / denominator;
            // float a = Mathf.Atan(ratio);
            // return a;
        }

        /// <summary>
        /// Return distance between two vector
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float GetDistance(Vector3 p1, Vector3 p2)
        {
            Vector3 v =p1-p2;
            return Mathf.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static List<Point> DeleteConcave(List<Point> points)
        {
            Point pInit = points[0];
            Point pivot = points[0];
            int i = 0;
            bool go = true;
            do
            {
                int prevIndex = i - 1 < 0 ? points.Count - 1 : i - 1;
                int nextIndex = i + 1 >= points.Count - 1 ? 0 : i + 1;
                float f = GetAngle(points[i].Position, points[prevIndex].Position, points[nextIndex].Position);
                if (f <= Mathf.PI )
                {
                    pivot = points[nextIndex];
                }
                else
                {
                    pInit = points[prevIndex];
                    points.Remove(points[i]);
                    go = false;
                }

                i++;
            } while (pivot != pInit && i <= points.Count-1);

            return points;

        }
    }
}