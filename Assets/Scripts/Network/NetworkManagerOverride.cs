using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerGameJam.Network
{
    public class NetworkManagerOverride : NetworkManager
    {
        private UnityTransport _transport;
        private SessionManager _sessionM;

        private void Awake()
        {
            _transport = GetComponent<UnityTransport>();
            _transport.SetConnectionData("51.159.6.4", 8989);
            /*
            _sessionM = new();
            OnClientConnectedCallback += (clientId) =>
            {
                _sessionM.AddConnection(clientId, Guid.NewGuid().ToString());
            };*/

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) // Is running headless
            {
                Debug.Log("Starting server");
                StartServer();
            }
        }
    }
}