using MultiplayerGameJam.Player;
using MultiplayerGameJam.Translation;
using MultiplayerGameJam.UI;
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

        public void DisplayExplanations()
        {
            UIManager.Instance.SetExplanationText(
                _type switch
                {
                    EmplacementType.Oars => Translate.Instance.Tr("oarsInfo", "F", "G"),
                    EmplacementType.Sail => Translate.Instance.Tr("sailInfo", "F"),
                    EmplacementType.Rudder => Translate.Instance.Tr("rudderInfo", "F", "G"),
                    _ => throw new NotImplementedException()
                }
            );
        }

        public void OnAction(MinigameKeyType key)
        {
            if (_type == EmplacementType.Oars)
            {
                _controller.AddRelativeVelocityServerRpc(Vector2.up);
                const float torqueValue = 90f;
                if (key == MinigameKeyType.F)
                {
                    _controller.AddTorqueServerRpc(torqueValue);
                }
                else if (key == MinigameKeyType.G)
                {
                    _controller.AddTorqueServerRpc(-torqueValue);
                }
            }
            else if (_type == EmplacementType.Sail)
            {
                if (key == MinigameKeyType.F)
                {
                    _controller.ToggleSailServerRpc();
                }
            }
            else if (_type == EmplacementType.Rudder)
            {
                if (key == MinigameKeyType.F)
                {
                    _controller.SteerRudderServerRpc(true);
                }
                else if (key == MinigameKeyType.G)
                {
                    _controller.SteerRudderServerRpc(false);
                }
            }
            else
            {
                throw new NotImplementedException($"Invalid type {_type}");
            }
        }
    }
}
