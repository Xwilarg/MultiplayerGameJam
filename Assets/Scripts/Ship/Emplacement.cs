using MultiplayerGameJam.Player;
using MultiplayerGameJam.Translation;
using MultiplayerGameJam.UI;
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

        public void OnAction(InputAction.CallbackContext e, MinigameKeyType key)
        {
            switch (_type)
            {
                case EmplacementType.Oars:
                    if (e.performed)
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
                    break;

                case EmplacementType.Sail:
                    if (e.performed && key == MinigameKeyType.F)
                    {
                        _controller.ToggleSailServerRpc();
                    }
                    break;

                case EmplacementType.Rudder:
                    // Old code
                    if (key == MinigameKeyType.F)
                    {
                        _controller.SteerRudderServerRpc(true);
                    }
                    else if (key == MinigameKeyType.G)
                    {
                        _controller.SteerRudderServerRpc(false);
                    }

                    // New Code:
                    if (e.phase == InputActionPhase.Started)
                    {

                    }
                    else if (e.phase == InputActionPhase.Canceled)
                    {

                    }
                    break;

                default:
                    throw new NotImplementedException($"Unknown emplacement {_type}");
            }
        }
    }
}
