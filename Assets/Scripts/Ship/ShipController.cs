using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _mov = new();

        public Emplacement Emplacement { set; get; }

        private void Start()
        {
            if (IsServer)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
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
