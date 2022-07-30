using MultiplayerGameJam.Player;
using System;
using UnityEngine;

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

        public void OnAction(MinigameKeyType key)
        {
            if (_type == EmplacementType.Oars)
            {
                _controller.AddRelativeVelocityServerRpc(Vector2.up * 100f);
            }
            else
            {
                throw new NotImplementedException($"Invalid type {_type}");
            }
        }
    }
}
