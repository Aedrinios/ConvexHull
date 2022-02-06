using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects;
using Statics;
using UnityEngine;

public class GarhamManager : MonoBehaviour
{
    public void compute()
    {
        if(!(CloudPointsManager.Instance.GetPoints() != null 
           && CloudPointsManager.Instance.GetPoints().Length > 0))
            CloudPointsManager.Instance.GenerateCloudsPoints();
        List<Point> points = CloudPointsManager.Instance.GetPoints().ToList();
        Point bc = CloudPointsManager.Instance.GetBarrycenter();

        LineRenderer convLr = CloudPointsManager.Instance.GetLineRenderer();
        
        points = GrahamScanStatic.Compute(points, bc);
        if (points.Count > 0)
        {
            convLr.positionCount = points.Count + 1;
            for (int i = 0; i < points.Count; i++)
                convLr.SetPosition(i, points[i].Position);
            convLr.SetPosition(points.Count, points[0].Position);
        }
    }
}
