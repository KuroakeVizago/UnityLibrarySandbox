using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Vizago.SceneManagement;

namespace Vizago.Networking
{
    public class NetworkLauncher : MonoBehaviourPunCallbacks
    {
        
        [SerializeField] private bool m_connectOnStarted;
        [SerializeField] private bool m_changeSceneOnConnected = false;
        [SerializeField, NaughtyAttributes.Scene] private string m_changeToScene;
        [Space]
        [SerializeField] private UnityEvent m_onConnectedToMasterServer;

        private void Start()
        {
            if (m_connectOnStarted)
            {
                CreateLobby();
            }
        }

        public virtual void CreateLobby(bool offlineLobby = false)
        {
            PhotonNetwork.ConnectUsingSettings(null, offlineLobby);
        }

        #region Event Callbacks
        
        public override void OnConnectedToMaster()
        {
            
            m_onConnectedToMasterServer?.Invoke();
            if (m_changeSceneOnConnected)
            {
                SceneManager.Instance.LoadScene(m_changeToScene);
            }

        }

        #endregion
        
    }
}
