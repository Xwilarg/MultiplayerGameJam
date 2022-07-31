﻿using MultiplayerGameJam.Ship;
using MultiplayerGameJam.SO;
using MultiplayerGameJam.UI;
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
        private PlayerInput _input;

        private NetworkVariable<Vector2> _mov = new();
        private ShipController _ship;

        private NetworkVariable<bool> _isOnEmplacement = new();
        public Emplacement CurrentEmplacement { set; get; }

        private void Start()
        {
            if (IsLocalPlayer)
            {
                Camera.main.GetComponent<CameraFollow>().Target = transform;
            }
            if (IsServer)
            {
                _rb = GetComponent<Rigidbody2D>();
                transform.parent = ShipManager.Instance.ShipParent;
                transform.localPosition = Vector2.zero;
            }
            _ship = ShipManager.Instance.ShipParent.GetComponent<ShipController>();
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (_isOnEmplacement.Value)
                {
                    _rb.velocity = Vector2.zero;
                    transform.position = CurrentEmplacement.transform.position;
                }
                else
                {
                    _rb.velocity = _mov.Value * Time.fixedDeltaTime * _info.Speed;
                }
                _rb.velocity += ShipManager.Instance.ShipParent.GetComponent<Rigidbody2D>().velocity;
            }
        }

        [ServerRpc]
        private void UpdatePositionServerRpc(Vector2 pos)
        {
            _mov.Value = pos;
        }

        [ServerRpc]
        private void SetIsOnEmplacementServerRpc(bool value)
        {
            _isOnEmplacement.Value = value;
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer)
            {
                var mov = value.ReadValue<Vector2>().normalized;
                UpdatePositionServerRpc(mov);
                if (mov.magnitude != 0f && _isOnEmplacement.Value)
                {
                    SetIsOnEmplacementServerRpc(false);
                    UIManager.Instance.SetExplanationText(string.Empty);
                }
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer && value.performed && CurrentEmplacement != null)
            {
                SetIsOnEmplacementServerRpc(true);
                CurrentEmplacement.DisplayExplanations();
            }
        }

        public void OnMinigame_F(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer && _isOnEmplacement.Value)
            {
                CurrentEmplacement.OnAction(value, MinigameKeyType.F);
            }
        }

        public void OnMinigame_G(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer && _isOnEmplacement.Value)
            {
                CurrentEmplacement.OnAction(value, MinigameKeyType.G);
            }
        }
    }
}
