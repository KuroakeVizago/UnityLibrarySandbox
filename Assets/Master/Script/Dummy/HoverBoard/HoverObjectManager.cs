using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class HoverObjectManager : MonoBehaviour
{
    [Header("Config:")] 
    [SerializeField] private Transform[] m_hoverPoints;
    [SerializeField] private LayerMask m_hoverOnLayer;
    [SerializeField] private float m_hoverRange = 5.0f;
    [SerializeField] private float m_forceValue = 75.0f;
    [Space]
    [SerializeField] protected bool m_isDebug = true;
    [SerializeField] private float m_updateInterval = 0.25f;
    
    public bool IsGrounded { get; private set; }
    
    private Rigidbody _rb;
    private float _updateTimer;

    private RaycastHit[] _hitInfo;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //IsGrounded = Physics.Raycast(transform.position, -Vector3.up, m_hoverRange * 2, m_hoverOnLayer);

        for (int i = 0; i < m_hoverPoints.Length; i++)
        {
            if (_hitInfo == null || _hitInfo.Length == 0)
                Array.Resize(ref _hitInfo, m_hoverPoints.Length);
            
            if (Physics.Raycast(m_hoverPoints[i].position, -Vector3.up, out var hitInfo, m_hoverRange, m_hoverOnLayer))
            {
                var forceValue = Vector3.up * ((1 - Mathf.Clamp01(hitInfo.distance / m_hoverRange)) * m_forceValue);
                _rb.AddForceAtPosition(forceValue, m_hoverPoints[i].position, ForceMode.Force);
            }
        }
    }

    private void OnDrawGizmos()
    {

        if (!m_isDebug) return;
        
        Gizmos.color = Color.red;
        foreach (var point in m_hoverPoints)
        {
            Gizmos.DrawSphere(point.position, 0.1f);
            if (Physics.Raycast(point.position, -Vector3.up, out var hitInfo, m_hoverRange, m_hoverOnLayer))
            {
                Gizmos.DrawLine(point.position, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, 0.1f);
            }
        }
    }
}
