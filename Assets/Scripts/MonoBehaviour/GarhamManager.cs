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
        CloudPointsManager.Instance.GenerateCloudsPoints();
        List<Point> points = CloudPointsManager.Instance.GetPoints().ToList();
        Point bc = CloudPointsManager.Instance.GetBarrycenter();
        for (int i = 0; i < points.Count; i++)
        {
            points[i].Angle = GrahamScanStatic.GetAngle(bc.Position, points[i].Position);
        }

        points = points.OrderBy(x => x.Angle).ToList();
        CloudPointsManager.Instance.SetPoint(points.ToArray());
        CloudPointsManager.Instance.ShowSort();
        
        LineRenderer convLr = CloudPointsManager.Instance.GetLineRenderer();
        convLr.positionCount = points.Count+1;
        for (int i = 0; i < points.Count; i++)
            convLr.SetPosition(i, points[i].Position);
        convLr.SetPosition(points.Count, points[0].Position);
    }
}
