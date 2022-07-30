using MultiplayerGameJam.Ship;
using MultiplayerGameJam.SO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerGameJam.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _mov = new();

        private void Start()
        {
            if (!IsLocalPlayer)
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
            if (IsServer)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
            transform.parent = ShipManager.Instance.ShipParent;
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                _rb.velocity = _mov.Value * Time.fixedDeltaTime * _info.Speed;
            }
        }

        [ServerRpc]
        private void UpdatePositionServerRpc(Vector2 pos)
        {
            _mov.Value = pos;
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer)
            {
                UpdatePositionServerRpc(value.ReadValue<Vector2>().normalized);
            }
        }
    }
}
