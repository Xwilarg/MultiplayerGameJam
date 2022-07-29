using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace MultiplayerGameJam.Network
{
    public class NetworkManagerOverride : NetworkManager
    {
        private UnityTransport _transport;
        private SessionManager _sessionM;

        private void Awake()
        {
            _transport = GetComponent<UnityTransport>();
            _transport.SetConnectionData("127.0.0.1", 8989);
            _sessionM = new();
            OnClientConnectedCallback += (clientId) =>
            {
                _sessionM.AddConnection(clientId, Guid.NewGuid().ToString());
            };
        }

        public void HostAction()
        {
            StartHost();
        }

        public void JoinAction()
        {
            StartClient();
        }
    }
}