using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _currentVelocity = new();
        private NetworkVariable<float> _currentAngularVelocity = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                _rb.velocity += _currentVelocity.Value;
                _rb.angularVelocity += _currentAngularVelocity.Value;

                _currentVelocity.Value = Vector2.zero;
                _currentAngularVelocity.Value = 0f;

                _rb.velocity /= 1.1f;
                _rb.angularVelocity /= 1.1f;
            }
        }

        [ServerRpc]
        public void AddRelativeVelocityServerRpc(Vector2 pos)
        {
            _currentVelocity.Value = (pos.y * transform.up + pos.x * transform.right).normalized * 10f;
        }

        [ServerRpc]
        public void AddTorqueServerRpc(float torque)
        {
            _currentAngularVelocity.Value = torque;
        }
    }
}
