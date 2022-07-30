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
        private ShipController _ship;

        private bool _isOnEmplacement;

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
            _ship = ShipManager.Instance.ShipParent.GetComponent<ShipController>();
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (_isOnEmplacement)
                {
                    _rb.velocity = Vector2.zero;
                    transform.position = _ship.Emplacement.transform.position;
                }
                else
                {
                    _rb.velocity = _mov.Value * Time.fixedDeltaTime * _info.Speed;
                }
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
                var mov = value.ReadValue<Vector2>().normalized;
                UpdatePositionServerRpc(mov);
                if (mov.magnitude != 0f && _isOnEmplacement)
                {
                    _isOnEmplacement = false;
                }
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer && value.performed && _ship.Emplacement != null)
            {
                _isOnEmplacement = true;
            }
        }
    }
}
