using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vizago
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Config:")] 
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_distance = 5.0f;

        private Vector3 _normalizedOffset;
        
        private void Start()
        {
            _normalizedOffset = Vector3.Normalize(transform.position - m_target.position);
        }

        private void LateUpdate()
        {

            transform.LookAt(m_target);
            
            transform.position = m_target.position + (_normalizedOffset * m_distance);
        }
    }
}
