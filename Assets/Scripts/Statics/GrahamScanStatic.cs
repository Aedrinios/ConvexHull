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
        /// return angle from 0 to 2PI for p relative to barrycenter
        /// </summary>
        /// <param name="bc">Barrycenter</param>
        /// <param name="p">Point</param>
        /// <returns></returns>
        public static float GetAngle(Vector3 bc, Vector3 p)
        {
            Vector3 v =p-bc;
            float angle = Mathf.Atan2(v.x, v.y);
            if (angle <= 0)
                angle = 2 * Mathf.PI + angle;
            return angle;
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
    }
}