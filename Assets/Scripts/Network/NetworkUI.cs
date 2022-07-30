using UnityEngine;

namespace MultiplayerGameJam.Network
{
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField]
        private NetworkManagerOverride _networkManager;

        public void Host()
        {
            _networkManager.StartHost();
            gameObject.SetActive(false);
        }

        public void Client()
        {
            _networkManager.StartClient();
            gameObject.SetActive(false);
        }
    }
}
