using MultiplayerGameJam.Ship;
using MultiplayerGameJam.SO;
using MultiplayerGameJam.Translation;
using MultiplayerGameJam.UI;
using System.Linq;
using TMPro;
using Unity.Collections;
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
        private Vector2 _direction = Vector2.up;
        private ShipController _ship;

        private NetworkVariable<bool> _isOnEmplacement = new();
        public Emplacement CurrentEmplacement { set; get; }

        private NetworkVariable<FixedString64Bytes> _name = new();
        private TMP_Text _nameContainer;

        private int _ignorePlayerLayer;
        private GameObject _closeShip;

        private SpriteRenderer _sr;
        private Animator _anim;

        private void Awake()
        {
            _ignorePlayerLayer = ~(1 << LayerMask.NameToLayer("Player"));
            _sr = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        private void Start()
        {
            _nameContainer = GetComponentInChildren<TMP_Text>();
            _name.OnValueChanged += (sender, e) =>
            {
                _nameContainer.text = _name.Value.ToString();
            };
            _nameContainer.text = _name.Value.ToString();

            if (IsLocalPlayer)
            {
                Camera.main.GetComponent<CameraFollow>().Target = transform;
                UpdateNameServerRpc(UIManager.Instance.PlayerName);
            }
            if (IsServer)
            {
                transform.position = GameObject.FindGameObjectWithTag("Spawn").transform.position;
            }
        }

        [ServerRpc]
        private void SetShipServerRpc(int id)
        {
            _ship = GameObject.FindGameObjectsWithTag("Ship").FirstOrDefault(x => x.GetComponent<ShipController>().Id.Value == id).GetComponent<ShipController>();
            transform.parent = _ship.transform;
            transform.localPosition = Vector2.zero;
        }

        [ServerRpc]
        private void UpdateNameServerRpc(string value)
        {
            _name.Value = new(value);
        }

        private void Update()
        {
            if (IsLocalPlayer && _ship == null)
            {
                var hit = Physics2D.Raycast(transform.position, _direction, 3f, _ignorePlayerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Ship"))
                {
                    UIManager.Instance.SetExplanationText(Translate.Instance.Tr("enterShip", "E"));
                    _closeShip = hit.collider.gameObject;
                }
                else
                {
                    UIManager.Instance.SetExplanationText(string.Empty);
                    _closeShip = null;
                }
            }
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
                if (_ship != null)
                {
                    _rb.velocity += _ship.GetComponent<Rigidbody2D>().velocity;
                }
            }
            if (_rb.velocity.x < 0f)
            {
                _sr.flipX = true;
                _anim.SetBool("IsWalking", true);
            }
            else if (_rb.velocity.x > 0f)
            {
                _sr.flipX = false;
                _anim.SetBool("IsWalking", true);
            }
            else
            {
                _anim.SetBool("IsWalking", _rb.velocity.y != 0f);
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
                if (mov.magnitude != 0f)
                {
                    _direction = mov.normalized;
                    if (_isOnEmplacement.Value)
                    {
                        SetIsOnEmplacementServerRpc(false);
                        UIManager.Instance.SetExplanationText(string.Empty);
                    }
                }
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer)
            {
                if (_ship == null && _closeShip != null)
                {
                    UIManager.Instance.SetExplanationText(string.Empty);
                    SetShipServerRpc(_closeShip.GetComponent<ShipController>()?.Id?.Value ?? _closeShip.GetComponentInParent<ShipController>().Id.Value);
                }
                else if (value.performed && CurrentEmplacement != null)
                {
                    SetIsOnEmplacementServerRpc(true);
                    CurrentEmplacement.DisplayExplanations();
                }
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

        public void OnMinigame_H(InputAction.CallbackContext value)
        {
            if (IsLocalPlayer && _isOnEmplacement.Value)
            {
                CurrentEmplacement.OnAction(value, MinigameKeyType.H);
            }
        }
    }
}
