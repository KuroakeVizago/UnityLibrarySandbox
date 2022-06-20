using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vizago.InputManager;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementComponent : MonoBehaviour
{
        
    [Header("Config:")] 
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private bool m_movementBasedOnCamera = true;
    [Space]
    [SerializeField] private AnimationCurve m_accelerationCurve;
    [SerializeField] private float m_accelerationSpeed = 1.5f;
    [SerializeField] private float m_movementSpeed = 3.4f;
    [SerializeField] private float m_gravityForce = 0.98f;
        
    public Vector2 MoveDirection => _inputDirection;
        
    public bool MovementEnable { get; set; } = true;

    public bool IsMoving => _inputDirection != Vector2.zero;
        
    private CharacterController _charController;
    private Vector2 _inputDirection;
    private float   _accelNormal;

    private void Awake()
    {

        _charController = GetComponent<CharacterController>();

    }

    private void Start()
    {
        //Input binding
        GameplayInputManager.Instance.CharacterInput.Movement.performed += inputContext =>
        {
            _inputDirection = inputContext.ReadValue<Vector2>();
        };

        GameplayInputManager.Instance.CharacterInput.Movement.canceled += inputContext =>
        {
            _inputDirection = Vector3.zero;
        };
    }

    private void Update()
    {
        if (MovementEnable)
        {
            _accelNormal += IsMoving
                ? (m_accelerationSpeed * Time.deltaTime)
                : -(m_accelerationSpeed * Time.deltaTime);
            _accelNormal = Mathf.Clamp01(_accelNormal);

            if (_charController)
            {

                Vector3 finMovement = Vector3.zero;

                if (IsMoving)
                {
                    if (!m_movementBasedOnCamera)
                    {
                        finMovement = new Vector3(_inputDirection.x, 0.0f, _inputDirection.y) * (m_movementSpeed * Time.deltaTime);
                    }
                    else if (m_movementBasedOnCamera && m_playerCamera)
                    {

                        finMovement =
                            m_playerCamera.transform.TransformDirection(new Vector3(_inputDirection.x, 0,
                                _inputDirection.y));
                        finMovement.y = 0;

                        finMovement.Normalize();
                        finMovement *= (Time.deltaTime * m_movementSpeed);
                    }
                }

                finMovement *= m_accelerationCurve.Evaluate(_accelNormal);
                finMovement.y -= m_gravityForce * Time.fixedDeltaTime;
                _charController.Move(finMovement);
            }
        }
    }
}