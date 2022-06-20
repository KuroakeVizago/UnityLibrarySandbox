using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vizago.InputManager
{
    /// <summary>
    /// An Input manager base
    /// Note: Implement singleton pattern for safety
    /// </summary>
    public abstract class InputManagerBase : MonoBehaviour
    {

        [SerializeField] private bool m_initAtStart;
        
        public InputSystem InputSystemInstance { get; private set; }

        private bool _isInitialized;
        
        private void OnEnable()
        {
            InputSystemInstance.Enable();
        }

        private void OnDisable()
        {
            InputSystemInstance.Disable();
        }

        protected virtual void Awake()
        {
            InputSystemInstance = new InputSystem();
            if (m_initAtStart)
                if (_isInitialized)
                    InitInput();
        }

        private void OnDestroy()
        {
            InputSystemInstance = null;
        }

        public virtual void InitInput()
        {
            _isInitialized = true;
        }
        
    }
}