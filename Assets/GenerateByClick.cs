using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateByClick : MonoBehaviour
{
    public static List<Point> points = new List<Point>();
    public GameObject pointPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Instantiate(pointPrefab, mousePos, Quaternion.identity, transform);
            points.Add(new Point(mousePos));
        }
    }
}