using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerGameJam.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private Rigidbody2D _rb;
        private Vector2 _mov;

        private void Start()
        {
            if (!IsLocalPlayer)
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
                return;
            }
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            _rb.velocity = _mov;
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            _mov = value.ReadValue<Vector2>().normalized;
        }
    }
}
