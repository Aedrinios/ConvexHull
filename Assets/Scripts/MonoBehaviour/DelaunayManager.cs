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
    private List<Tuple<Point,Point>> segments;
    private int amountTriangulated;
    private void Start()
    {
        if (!TryGetComponent(out cpm))
        {
            
        }

        lines = new List<LineRenderer>();
        segments = new List<Tuple<Point,Point>>();
        amountTriangulated = 0;
    }
    
    public void DelaunayStart()
    {
        IncrementalTriangulation(cpm.GetPoints());
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
        
        Triangulate(sortedPoints[0], sortedPoints[1], sortedPoints[2]);
        amountTriangulated = 3;
        //on parcours tous les points pas encore triangulés
        for (int i = amountTriangulated; i < sortedPoints.Length; ++i)
        {
            Debug.Log("j'essaie de trianguler le point " + i);
            //et on teste toutes les combinaisons de segments entre le point actuel et les points déja triangulés
            Point a = sortedPoints[i]; //celui qu'on veut trianguler

            List<Point> goodPoints = new List<Point>();
            for (int j = 0; j < amountTriangulated; ++j)
            {
                Debug.Log("je tente le point " + j);
                Point b = sortedPoints[j]; //celui qu'on veut tester si il croise un segment des points triangulés
                bool intersectsWithAnEdge = false;
                foreach (var segment in segments)
                {
                    //le segment déjà créé qu'on veut tenter d'intersecter
                    Point c = segment.Item1;
                    Point d = segment.Item2;
                    if (LinesIntesect(a, b, c, d))
                    {
                        intersectsWithAnEdge = true;
                        break;
                    }
                    
                }

                //si on a croisé aucun edge existant
                if (!intersectsWithAnEdge)
                {
                    goodPoints.Add(b);
                }

            }
            Debug.Log("good points : " + goodPoints.ToString());
            Triangulate(a, goodPoints[0], goodPoints[1]);
            amountTriangulated++;
        }
        //StartCoroutine(showPointsIncrementally());
    }

    private bool LinesIntesect(Point a, Point b, Point c, Point d)
    {
        //ligne 1 : [A:B]
        //ligne 2 : [C:D]
        
        float I1min = Mathf.Min(a.Position.x,b.Position.x);
        float I1max = Mathf.Max(a.Position.x,b.Position.x);
        float I2min = Mathf.Min(c.Position.x,d.Position.x);
        float I2max = Mathf.Max(c.Position.x,d.Position.x);
        float Iamin = Mathf.Max(I1min,I2min);
        float Iamax = Mathf.Min(I1max,I2max);
        
        if (I1max < I2min) return false;
        
        //f1(x) = A1*x + b1 = y
        //f2(x) = A2*x + b2 = y
        float A1 = (a.Position.y-b.Position.y)/(a.Position.x-b.Position.x);
        float A2 = (c.Position.y-d.Position.y)/(c.Position.x-d.Position.x);
        
        if (Math.Abs(A1 - A2) < 0.0001f) return false; //si les deux valeurs sont tres proches elles sont considérées parallèles
        float b1 = a.Position.y - A1 * a.Position.x;
        float b2 = c.Position.y - A2 * c.Position.x;

        float Xa = (b2-b1)/(A2-A1);
        float Ya = A1 * Xa + b1;

        if ((Xa < Iamin) || (Xa > Iamax)) return false;
        
        return true;
    }
    private void Triangulate(Point a, Point b, Point c)
    {
        LineRenderer lr = Instantiate(lrPrefab,Vector3.zero, Quaternion.identity);
        lr.positionCount = 3;
        lr.SetPosition(0,a.Position);
        lr.SetPosition(1,b.Position);
        lr.SetPosition(2,c.Position);
        
        lines.Add(lr);
        
        segments.Add(new Tuple<Point, Point>(a,b));
        segments.Add(new Tuple<Point, Point>(b,c));
        segments.Add(new Tuple<Point, Point>(c,a));
    }

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
