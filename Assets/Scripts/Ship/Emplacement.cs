using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerGameJam.Ship
{
    public class Emplacement : MonoBehaviour
    {
        [SerializeField]
        private EmplacementType _type;

        private ShipController _controller;

        private void Awake()
        {
            _controller = transform.parent.GetComponent<ShipController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _controller.Emplacement = this;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            _controller.Emplacement = null;
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (_type == EmplacementType.Oars)
            {
                if (value.performed)
                {
                    var x = value.ReadValue<Vector2>().x;
                }
            }
            else
            {
                throw new NotImplementedException($"Invalid type {_type}");
            }
        }
    }
}
