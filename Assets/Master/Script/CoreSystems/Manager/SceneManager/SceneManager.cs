using System;
using System.Collections;
using UnityEngine;
using Vizago.HUD;

namespace Vizago.SceneManagement
{
    public class SceneManager : MonoBehaviour
    {
        
        [Header("Component:")]
        [SerializeField] private LoadingScreenUI m_loadingScreen;
        [Space]
        [Header("Misc:")]
        [SerializeField] private float m_sceneDefaultLoadTime = 4.5f;
        
        public static SceneManager Instance { get; private set; }
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(this);
            }

        }

        public void LoadScene(string sceneToLoad)
        {
            void LoadOnScreenVisible(bool visibility)
            {
                if (visibility) 
                    StartCoroutine(LoadAsync(sceneToLoad));

                m_loadingScreen.OnVisibilityChange -= LoadOnScreenVisible;
            }

            m_loadingScreen.OnVisibilityChange += LoadOnScreenVisible;
            
            if (m_loadingScreen)
                m_loadingScreen.SetVisibility(true);
            
        }

        private IEnumerator LoadAsync(string sceneName)
        {
            m_loadingScreen.BindProgressValue = 0;
            
            AsyncOperation asyncScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            asyncScene.allowSceneActivation = false;

            while (!asyncScene.isDone)
            {

                if (m_loadingScreen && m_loadingScreen.BindProgressValue < asyncScene.progress)
                {
                    yield return new WaitForSeconds(Time.deltaTime); 
                    m_loadingScreen.BindProgressValue += Time.deltaTime;
                }
                
                if (asyncScene.progress >= 0.9f && (m_loadingScreen.BindProgressValue >= asyncScene.progress))
                {

                    m_loadingScreen.BindProgressValue = 1.0f;
                    
                    //Giving phantom delay on the loading screen
                    yield return new WaitForSeconds(m_sceneDefaultLoadTime);
                    
                    asyncScene.allowSceneActivation = true;

                    m_loadingScreen.SetVisibility(false);

                    yield return null;

                }
            }
        }
    }
}
