using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.UIElements;

public class DelaunayManager : MonoBehaviour
{
    public LineRenderer lrPrefab;
    private CloudPointsManager cpm;
    private Point[] sortedPoints;

    private List<LineRenderer> lines;
    private List<Tuple<int,int>> segments;
    private int amountTriangulated;

    private bool intersectTest;
    private GameObject[] intersectTestPoints;
    private LineRenderer[] intersectTestLr;
    private void Start()
    {
        if (!TryGetComponent(out cpm))
        {
            
        }

        lines = new List<LineRenderer>();
        segments = new List<Tuple<int,int>>();
        amountTriangulated = 0;
        intersectTestPoints = new GameObject[4];
        intersectTestLr = new LineRenderer[2];
        intersectTest = false;
    }
    
    public void DelaunayStart()
    {
        IncrementalTriangulation(cpm.GetPoints());
    }
    public void IntersectTest()
    {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        intersectTestPoints[0] = Instantiate(primitive, new Vector3(2, 7, 0), Quaternion.identity);
        intersectTestPoints[1] = Instantiate(primitive, new Vector3(13, 1, 0), Quaternion.identity);
        intersectTestPoints[2] = Instantiate(primitive, new Vector3(15, 9, 0), Quaternion.identity);
        intersectTestPoints[3] = Instantiate(primitive, new Vector3(6, 3, 0), Quaternion.identity);
        intersectTestLr[0] = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
        intersectTestLr[0].positionCount = 2;
        intersectTestLr[1] = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
        intersectTestLr[1].positionCount = 2;
        intersectTest = true;
    }

    private void FixedUpdate()
    {
        if (intersectTest)
        {
            Vector3 a = intersectTestPoints[0].transform.position;
            intersectTestLr[0].SetPosition(0,a);
            Vector3 b = intersectTestPoints[1].transform.position;
            intersectTestLr[0].SetPosition(1,b);
            Vector3 c = intersectTestPoints[2].transform.position;
            intersectTestLr[1].SetPosition(0,c);
            Vector3 d = intersectTestPoints[3].transform.position;
            intersectTestLr[1].SetPosition(1,d);
            if (doIntersect(a, b, c, d))
            {
                Debug.Log("Intersection detected");
            }
            else
            {
                Debug.Log("No intersection");
            }
            
        }
    }

    public void IncrementalTriangulation(Point[] points)
    {
        Debug.Log("Youhou je suis appelé !");
        if (points == null) return;
        if (points.Length <= 0) return;
        
        Debug.Log("Debut delaunay !");
        sortedPoints = new Point[points.Length];
        for(int i = 0; i < points.Length; i++)
        {
            sortedPoints[i] = points[i];
        }
        Array.Sort(sortedPoints);
        
        Triangulate(0, 1, 2);
        amountTriangulated = 3;
        //on parcours tous les points pas encore triangulés
        for (int i = amountTriangulated; i < sortedPoints.Length; ++i)
        {
            //Debug.Log("j'essaie de trianguler le point " + i);
            //et on teste toutes les combinaisons de segments entre le point actuel et les points déja triangulés
            Point a = sortedPoints[i]; //celui qu'on veut trianguler

            List<int> goodPoints = new List<int>();// la liste de tous les points compatibles
            for (int j = 0; j < amountTriangulated; ++j)
            {
                Point b = sortedPoints[j]; //celui qu'on veut tester si il croise un segment des points triangulés
                bool intersectsWithAnEdge = false;
                for (int k = 0; k < segments.Count; ++k)
                {
                    Tuple<int, int> segment = segments[k];
                    if (j == segment.Item1 || j == segment.Item2) continue;//si b est l'une des extrémitées du segment qu'on cherche à tester, alors b est visible par a
                    
                    //le segment déjà créé qu'on veut tenter d'intersecter
                    Point c = sortedPoints[segment.Item1];
                    Point d = sortedPoints[segment.Item2];

                    //Debug.Log("Test d'intersection entre [" + i + ", " + j + "] et le segment "+ k +" : [" + c.Position.ToString() + ", " +  d.Position.ToString() + "]");
                    if (doIntersect(a.Position, b.Position, c.Position, d.Position))//si le segment [a,b] croise [c,d] alors b n'est pas un point visible
                    {
                        //Debug.Log("Intersection détecté");
                        intersectsWithAnEdge = true;
                        break;
                    }
                    
                }

                //si on a croisé aucun edge existant
                if (!intersectsWithAnEdge)
                {
                    //Debug.Log("point b at index " + j + "didn't intersect. Adding its position as a good point.");
                    goodPoints.Add(j);
                }

                //Debug.Log("goodPoints size : " + goodPoints.Count);
            }
            Debug.Log("good points size : " + goodPoints.Count);
            for (int z = 1; z < goodPoints.Count; ++z)
            {
                Triangulate(i, goodPoints[z-1], goodPoints[z]);
            }
            amountTriangulated++;
        }
        //StartCoroutine(showPointsIncrementally());
    }
    private void Triangulate(int a, int b, int c)
    {
        LineRenderer lr = Instantiate(lrPrefab,Vector3.zero, Quaternion.identity);
        lr.positionCount = 3;
        lr.SetPosition(0,sortedPoints[a].Position);
        lr.SetPosition(1,sortedPoints[b].Position);
        lr.SetPosition(2,sortedPoints[c].Position);
        
        lines.Add(lr);
        
        segments.Add(new Tuple<int, int>(a,b));
        segments.Add(new Tuple<int, int>(b,c));
        segments.Add(new Tuple<int, int>(c,a));
    }
    
// The main function that returns true if line segment 'p1q1'
// and 'p2q2' intersect.
    static Boolean doIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        // Find the four orientations needed for general and
        // special cases
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);
 
        // General case
        if (o1 != o2 && o3 != o4)
            return true;
 
        // Special Cases
        // p1, q1 and p2 are collinear and p2 lies on segment p1q1
        if (o1 == 0 && onSegment(p1, p2, q1)) return true;
 
        // p1, q1 and q2 are collinear and q2 lies on segment p1q1
        if (o2 == 0 && onSegment(p1, q2, q1)) return true;
 
        // p2, q2 and p1 are collinear and p1 lies on segment p2q2
        if (o3 == 0 && onSegment(p2, p1, q2)) return true;
 
        // p2, q2 and q1 are collinear and q1 lies on segment p2q2
        if (o4 == 0 && onSegment(p2, q1, q2)) return true;
 
        return false; // Doesn't fall in any of the above cases
    }
    // To find orientation of ordered triplet (p, q, r).
// The function returns following values
// 0 --> p, q and r are collinear
// 1 --> Clockwise
// 2 --> Counterclockwise
    static int orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
        // for details of below formula.
        float val = (q.y - p.y) * (r.x - q.x) -
                  (q.x - p.x) * (r.y - q.y);
 
        if (Math.Abs(val) < 0.001f) return 0; // collinear
 
        return (val > 0)? 1: 2; // clock or counterclock wise
    }
    // Given three collinear points p, q, r, the function checks if
// point q lies on line segment 'pr'
    static Boolean onSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
            q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
            return true;
 
        return false;
    }
    
    
    
    // DEBUG
    

    private IEnumerator showPointsIncrementally()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        foreach (var point in sortedPoints)
        {
            point.Go.SetActive(false);
        }
        foreach (var point in sortedPoints)
        {
            point.Go.SetActive(true);
            yield return wait;
        }
        yield return null;
    }
}
