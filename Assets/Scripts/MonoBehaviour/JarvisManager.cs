using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarvisManager : MonoBehaviour
{
    private CloudPointsManager cpm;
    private void Start()
    {
        if (!TryGetComponent(out cpm))
        {
            
        }
    }

    public void JarvisStart()
    {
        LineRenderer convLr = cpm.GetLineRenderer();
        Vector3[] javisVec = JavisWalkStatic.Calculate(cpm.GetPoints());
        convLr.positionCount = javisVec.Length;
        for (int i = 0; i < javisVec.Length; i++)
            convLr.SetPosition(i, javisVec[i]);
    }
}
