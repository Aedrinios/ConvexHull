using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CloudPointsStatic
{
    private static int xMax = 18;
    private static int xMin = 0;
    private static int yMax = 10;
    private static int yMin = 0;
    
    public static  Vector3[]  Create2DCloudPoints(int nbPoints)
    {
        Vector3[] points = new Vector3[nbPoints];
        for (int i = 0; i < nbPoints; i++)
        {
            points[i] = new Vector3(
                Random.Range((float)xMin, (float)xMax), 
                Random.Range((float)yMin, (float)yMax),
                0f);
        }
        return points;
    }
}
