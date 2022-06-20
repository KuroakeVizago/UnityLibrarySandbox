using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationDrawer : MonoBehaviour
{
    
    [SerializeField] private float m_width = 4;
    [SerializeField] private float m_depth = 4;
    
    
    private void OnDrawGizmos()
    {
        Queue<Vector2> queueni;
        
            
        
        Gizmos.color = Color.yellow;
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_depth; y++)
            {
                Vector3 drawPoint = transform.position + new Vector3(x, y);
                Gizmos.DrawSphere(drawPoint, 0.05f);
            }
        }
    }
}
