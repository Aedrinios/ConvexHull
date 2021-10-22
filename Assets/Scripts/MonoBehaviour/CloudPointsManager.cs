using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudPointsManager : MonoBehaviour
{
    
    [SerializeField] GameObject pointGo;
    [SerializeField] Transform container;
    // Start is called before the first frame update
    void Start()
    {

        Vector3[] points = CloudPointsStatic.Create2DCloudPoints(500);
        for (int i = 0; i < points.Length; i++)
        {
            Instantiate(pointGo, points[i], Quaternion.identity, container);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
