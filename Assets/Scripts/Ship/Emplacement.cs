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
                UIManager.Instance.SetExplanationText(string.Empty);
            }
        }

        public void DisplayExplanations()
        {
            UIManager.Instance.SetExplanationText(_type switch
            {
                EmplacementType.Oars => Translate.Instance.Tr("oarsInfo", "F", "G"),
                _ => throw new NotImplementedException()
            });
        }

        public void OnAction(MinigameKeyType key)
        {
            if (_type == EmplacementType.Oars)
            {
                _controller.AddRelativeVelocityServerRpc(Vector2.up);
                const float torqueValue = 25f;
                if (key == MinigameKeyType.F)
                {
                    _controller.AddTorqueServerRpc(torqueValue);
                }
                else if (key == MinigameKeyType.G)
                {
                    _controller.AddTorqueServerRpc(-torqueValue);
                }
            }
            else
            {
                throw new NotImplementedException($"Invalid type {_type}");
            }
        }
    }
}
