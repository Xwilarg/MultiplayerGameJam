using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _mov = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                _rb.velocity /= 1.1f;
                _rb.angularVelocity /= 1.1f;
            }
        }

        [ServerRpc]
        public void AddRelativeVelocityServerRpc(Vector2 pos)
        {
            _rb.velocity = (pos.y * transform.up + pos.x * transform.right).normalized * 10f;
        }

        [ServerRpc]
        public void AddTorqueServerRpc(float torque)
        {
            _rb.angularVelocity = torque;
        }
    }
}
