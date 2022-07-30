using MultiplayerGameJam.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerGameJam.Ship
{
    public class Emplacement : MonoBehaviour
    {
        [SerializeField]
        private EmplacementType _type;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<PlayerController>().CurrentEmplacement = this;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<PlayerController>().CurrentEmplacement = null;
            }
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
