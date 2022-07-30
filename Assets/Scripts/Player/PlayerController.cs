using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerGameJam.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private Rigidbody2D _rb;

        private void Start()
        {
            if (IsLocalPlayer)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
            else
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
        }

        [ServerRpc]
        private void UpdatePositionServerRpc(Vector2 pos)
        {
            _rb.velocity = pos;
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
