using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CurveDrawer : MonoBehaviour
{
    [SerializeField] private Transform[] drawPoints;
    [SerializeField] private float speed = 2.0f;

    private Vector3[] pointsPosition;

    private Vector3 pointPos = Vector3.zero;

    private void Start()
    {
        pointsPosition = new Vector3[drawPoints.Length - 1];
    }

    private void Update()
    {

        var t = Mathf.Abs(Mathf.Sin(Time.time));
        
        for (int i = 1; i < drawPoints.Length; i++)
        {
            pointsPosition[i - 1] = Vector3.Lerp(drawPoints[i - 1].position, drawPoints[i].position, t);    
        }

        for (int i = 1; i < pointsPosition.Length; i++)
        {
            pointPos = Vector3.Lerp(pointsPosition[i - 1], pointsPosition[i], t);
        }

    }

    private void OnDrawGizmos()
    {

        if (pointsPosition == null)
            return;
        
        Gizmos.color = Color.grey;
        for (int i = 0; i < pointsPosition.Length; i++)
        {
            Gizmos.DrawSphere(pointsPosition[i], 0.2f);    
        }
        
        for (int i = 1; i < pointsPosition.Length; i++)
        {
            Gizmos.DrawLine(pointsPosition[i - 1], pointsPosition[i]);
        }

        Gizmos.color = Color.grey;
        for (int i = 1; i < drawPoints.Length; i++)
        {
            Gizmos.DrawLine(drawPoints[i - 1].position, drawPoints[i].position);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(pointPos, 0.2f);

        

    }
}
