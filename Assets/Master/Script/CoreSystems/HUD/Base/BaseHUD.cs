using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Vizago.HUD
{
    public class BaseHUD : MonoBehaviour
    {
        [Header("Config:")] 
        [SerializeField] private bool m_hideAtStart = false;
        [SerializeField] private bool m_fadeTransition = true;
        [SerializeField, Min(0.1f)] private float m_transitionSpeed = 1.0f;
        [SerializeField] private CanvasGroup m_hudCanvasGroup;

        #region Public Property
        
        /// <summary>
        /// Only to read the HUD visibility, change the HUD Visibility using SetVisibility() function instead!
        /// </summary>
        public bool HUDVisibility => m_hudCanvasGroup.alpha > 0;

        /// <summary>
        /// Whether changing the HUD visibility is using fade transition or instant change
        /// </summary>
        public bool FadeTransition
        {
            get => m_fadeTransition;
            set => m_fadeTransition = value;
        }
        
        /// <summary>
        /// The transition speed if using FadeTransition.
        /// </summary>
        public float TransitionSpeed 
        { 
            get => m_transitionSpeed;
            set => m_transitionSpeed = Mathf.Clamp( value, 0.1f, Mathf.Infinity);
        }
        
        public bool IsChangingVisibility { get; private set; }

        /// <summary>
        /// The current visibility of a HUD. Can be change using SetVisibility() Function
        /// </summary>
        public bool Visibility { 
            get => _visibility;
            private set
            {
                _visibility = value;
                OnVisibilityChange?.Invoke(value);
            }
        }

        public UnityAction<bool> OnVisibilityChange { get; set; }

        #endregion

        /// <summary>
        /// Always use Visibility public property to change the value
        /// </summary>
        private bool _visibility;
        private bool _targetVisibility;

        protected void Awake()
        {
            InitHUD();
        }

        protected void Start()
        {
            
            
            if (m_hideAtStart)
            {
                if (m_hudCanvasGroup)
                {
                    _visibility = false;
                    m_hudCanvasGroup.alpha = 0;
                    OnVisibilityChange?.Invoke(_visibility);
                }
                else
                    Debug.Log("HUD Component Missing canvas group");
            }
            
            StartHUD();
            
        }

        protected void LateUpdate()
        {
            
            // Change the HUD Visibility Function
            if (IsChangingVisibility)
            {
                //Change using Fade Effect or not
                if (FadeTransition)
                {
                    if (_targetVisibility)
                    {
                        if (m_hudCanvasGroup.alpha < 1)
                            m_hudCanvasGroup.alpha += Time.deltaTime * m_transitionSpeed;
                        else
                        {
                            IsChangingVisibility = false;
                            if (Visibility != _targetVisibility)
                                Visibility = _targetVisibility;
                        }
                    }
                    else
                    {
                        if (m_hudCanvasGroup.alpha > 0)
                            m_hudCanvasGroup.alpha -= Time.deltaTime * m_transitionSpeed;
                        else
                        {
                            IsChangingVisibility = false;
                            if (Visibility != _targetVisibility)
                                Visibility = _targetVisibility;
                        }
                    }
                }
                else
                {
                    // Instant Change
                    if (_targetVisibility)
                    {
                        if (m_hudCanvasGroup.alpha < 1)
                            m_hudCanvasGroup.alpha = 1;
                        else
                        {
                            IsChangingVisibility = false;
                            if (Visibility != _targetVisibility)
                                Visibility = _targetVisibility;
                        }
                    }
                    else
                    {
                        if (m_hudCanvasGroup.alpha > 0)
                            m_hudCanvasGroup.alpha = 0;
                        else
                        {
                            IsChangingVisibility = false;
                            if (Visibility != _targetVisibility)
                                Visibility = _targetVisibility;
                        }
                    }
                }
            }
            
            UpdateHUD();
            
        }

        #region VisibilityFunction
        
        public void SetVisibility(bool visibility, bool fadeTransition = true)
        {
            if (Visibility != visibility)
                _targetVisibility = visibility;
            
            FadeTransition = fadeTransition;
            IsChangingVisibility = true;
        }
        
        public void SetVisibility(bool visibility, float fadeTransitionSpeed, bool fadeTransition = true)
        {
            if (Visibility != visibility)
                _targetVisibility = visibility;
            
            FadeTransition = fadeTransition;
            TransitionSpeed = fadeTransitionSpeed;
            IsChangingVisibility = true;
        }

        #endregion
        
        #region ChildFunction

        protected virtual void InitHUD() { }

        protected virtual void StartHUD() { }
        
        protected virtual void UpdateHUD() { }
        
        #endregion
    }
}
