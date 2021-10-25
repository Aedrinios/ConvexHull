using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public class CloudPointsManager : MonoBehaviour
{
    
    [SerializeField] GameObject pointGo;
    [SerializeField] Transform container;
    
    private LineRenderer convLr;
    
    private Point[] points;

    private bool doSomething = false;

    private int pointIndex = 0;

    private void Awake()
    {
        if (!TryGetComponent(out convLr))
        {
            throw new System.Exception("Wsh met un LineRenderer sur l'enfant de l'enfant stp");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        points = CloudPointsStatic.Create2DCloudPoints(50);
        for (int i = 0; i < points.Length; i++)
        {
            points[i].Go = Instantiate(pointGo, points[i].Position, Quaternion.identity, container);
        }

        Vector3[] javisVec = JavisWalkStatic.Calculate(points);
        convLr.positionCount = javisVec.Length;
        for (int i = 0; i < javisVec.Length; i++)
            convLr.SetPosition(i, javisVec[i]);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            doSomething = true;
        }
    }

    private void FixedUpdate()
    {
        // if (doSomething)
        // {
        //     JavisWalkStatic.Calculate()
        // }
    }
}
