using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(HoverObjectManager))]
public class HoverBoardController : MonoBehaviour
{
    [Header("Config:")] 
    [SerializeField] private float m_moveSpeed = 10.0f;
    [SerializeField] private float m_turnSpeed = 65.0f;

    private HoverObjectManager _hoverManager;
    private Rigidbody _rb;

    private void Start()
    {
        _hoverManager = GetComponent<HoverObjectManager>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        float hoverDirection =
            Keyboard.current.wKey.isPressed ? 1
            : Keyboard.current.sKey.isPressed ? -1
            : 0;

        _rb.AddForce(transform.TransformDirection(new Vector3(0, 0, hoverDirection) * (m_moveSpeed * 100)));

        float torqueDirection =
            Keyboard.current.aKey.isPressed ? -1
            : Keyboard.current.dKey.isPressed ? 1
            : 0;

        _rb.AddTorque(transform.up * (torqueDirection * m_turnSpeed));

    }

}
