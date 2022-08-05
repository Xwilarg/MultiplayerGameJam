using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace MultiplayerGameJam.Network
{
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField]
        private NetworkManagerOverride _networkManager;

        public void HostDebug()
        {
            _networkManager.GetComponent<UnityTransport>().ConnectionData.Address = "127.0.0.1";
            _networkManager.StartHost();
            gameObject.SetActive(false);
        }

        public void ClientDebug()
        {
            _networkManager.GetComponent<UnityTransport>().ConnectionData.Address = "127.0.0.1";
            _networkManager.StartClient();
            gameObject.SetActive(false);
        }

        public void Client()
        {
            _networkManager.StartClient();
            gameObject.SetActive(false);
        }
    }
}
