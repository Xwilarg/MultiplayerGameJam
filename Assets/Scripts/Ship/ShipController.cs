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
                _rb.velocity = _mov.Value * Time.fixedDeltaTime * 100f;
            }
        }

        [ServerRpc]
        public void AddVelocityServerRpc(Vector2 pos)
        {
            _mov.Value = pos;
        }
    }
}
