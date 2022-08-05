using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        private static int _idStatic = 0;

        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _mov = new();

        //Environment properties
        private Vector2 _windDirection;
        private float _windMagnitude;
        private const float windAccelerationCoeff = 0.1f;
        private bool _isAnchorDeployed = true;

        //Ship properties
        private NetworkVariable<bool> _sailLowered = new();
        private NetworkVariable<float> _rudderTorqueCoefficient = new();
        private const float _maxShipVelocity = 9f;
        private const float _maxRudderTorqueCoefficient = 7f;

        public NetworkVariable<int> Id { private set; get; } = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _windDirection = Vector2.up;
            _windMagnitude = 1f;
            _sailLowered.Value = false;
            _rudderTorqueCoefficient.Value = 0f;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                Id.Value = _idStatic++;
            }
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (_sailLowered.Value)
                {
                    accelerateBySailServerRpc();
                }
                _rb.angularVelocity += _rudderTorqueCoefficient.Value;
                _rb.velocity /= 1.002f * (_isAnchorDeployed ? 10f : 1f);
                _rb.angularVelocity /= 1.25f;
            }
        }

        [ServerRpc(RequireOwnership = false)] // TODO: Bad idea
        public void AddRelativeVelocityServerRpc(Vector2 pos)
        {
            _rb.velocity = (pos.y * transform.up + pos.x * transform.right).normalized * 10f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void InvertAnchorServerRpc()
        {
            _isAnchorDeployed = !_isAnchorDeployed;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddTorqueServerRpc(float torque)
        {
            _rb.angularVelocity = torque;
        }

        //Raise or lower sail
        [ServerRpc(RequireOwnership = false)]
        public void ToggleSailServerRpc()
        {
            _sailLowered.Value = !(_sailLowered.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        private void accelerateBySailServerRpc()
        {
            Vector2 shipDirection = new(
                -Mathf.Sin(_rb.rotation * Mathf.Deg2Rad),
                Mathf.Cos(_rb.rotation * Mathf.Deg2Rad)
            );
            //Calculate angle between sailing direction and wind direction
            float sailingAngle =
                Mathf.Acos(Vector2.Dot(shipDirection, _windDirection * -1)) * 180 / Mathf.PI;

            //Only sail when not headwind (i.e. outside No-Go Zone)
            if (sailingAngle > 45f)
            {
                float newSpeed = _rb.velocity.magnitude + windAccelerationCoeff;
                //Ensure that velocity does not exceed a capped value
                if (newSpeed > _maxShipVelocity)
                {
                    newSpeed = _maxShipVelocity;
                }
                //Accelerate the ship
                _rb.velocity = shipDirection * newSpeed;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SteerRudderServerRpc(bool direction)
        {
            if (direction && _rudderTorqueCoefficient.Value < _maxRudderTorqueCoefficient)
            {
                _rudderTorqueCoefficient.Value += 0.03f;
            }
            else if (_rudderTorqueCoefficient.Value > -_maxRudderTorqueCoefficient)
            {
                _rudderTorqueCoefficient.Value -= 0.03f;
            }
        }
    }
}
