using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

namespace Statics
{
    public static class GrahamScanStatic
    {
        public static List<Point> Compute(List<Point> points)
        {
            Point barryCenter = new Point();
            for (int i = 0; i < points.Count; i++)
            {
                barryCenter.Position += points[i].Position;
            }
            barryCenter.Position /= points.Count;
            
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Angle = GrahamScanStatic.GetAngle(barryCenter.Position, points[i].Position);
            }
            points = points.OrderBy(x => x.Angle).ToList();
            
            return DeleteConcave(points);
        }
        public static List<Point> SortPoints(List<Point> points, Point bc = null)
        {
            if (bc == null)
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
            Vector3 v = side2 - side1;
            float angle = Mathf.Atan2(v.x, v.y);
            if (angle <= 0)
                angle = 2 * Mathf.PI + angle;
            return angle;
        }

        /// <summary>
        /// Calculate between three points
        /// return angle from 0 to 2PI for p1 relative to p2
        /// </summary>
        /// <param name="p0">Pivot</param>
        /// <param name="p1">p-1</param>
        /// <param name="p2">p+1</param>
        /// <returns>Radians</returns>
        public static float GetAngle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Vector2.SignedAngle(p3 - p1, p2 - p1);
        }

        /// <summary>
        /// Return distance between two vector
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float GetDistance(Vector3 p1, Vector3 p2)
        {
            Vector3 v = p1 - p2;
            return Mathf.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static List<Point> DeleteConcave(List<Point> points)
        {
           
            LinkedList<Point> tmp = new LinkedList<Point>(points);
            int i = 0;
            LinkedListNode<Point> pInit = tmp.First;
            LinkedListNode<Point> pivot = pInit;
            bool go = true;

            do
            {
                float rad = GetAngle(pivot.Value.Position, GePrevious(pivot).Value.Position,
                    GetNext(pivot).Value.Position);
                //         L[previous].Position - pivot.Position);
                if (rad > 180 || rad < 0)
                {
                    pivot = GetNext(pivot);
                    go = true;
                }
                else
                {
                    pInit = GePrevious(pivot);
                    tmp.Remove(pivot);
                    pivot = pInit;
                    go = false;
                }
            } while ((pivot.Value.Position != pInit.Value.Position || go == false));

            points = new List<Point>(tmp);
            
            return points;
        }

        private static LinkedListNode<T> GetNext<T>(LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        private static LinkedListNode<T> GePrevious<T>(LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }
    }
}