using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vizago.HUD
{
    public class LoadingScreenUI : BaseHUD
    {

        [Space] 
        [Header("Required Object:")] 
        [SerializeField] private Canvas m_loadingCanvas;
        [Space]
        [Header("Screen Visual:")]
        [SerializeField] private Image m_progressFillBar;
        [SerializeField] private GameObject[] m_loadScreenVisualEffect;
        
        /// <summary>
        /// Set this value to update the UI Loading Progress Bar.
        /// Note: The progress value is between 0 to 1
        /// </summary>
        public float BindProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = Mathf.Clamp01(value);
                if (m_progressFillBar)
                    m_progressFillBar.fillAmount = _progressValue;
                else
                    Debug.LogWarning(nameof(m_progressFillBar) + " have no reference, so progress bar will not updated");
            }
        }

        private float _progressValue;

        protected override void InitHUD()
        {
            OnVisibilityChange += visibility =>
            {
                foreach (var visualEffect in m_loadScreenVisualEffect)
                {
                    visualEffect.SetActive(visibility);
                }
            };

            if (!m_loadingCanvas.worldCamera)
                m_loadingCanvas.worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
        }
    }
}