using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        //Constants
        //Max Ship velocity restriction
        private const float _maxShipVelocity = 9f;
        //Max RudderAngle
        private const float _maxRudderAngle = 60f * Mathf.Deg2Rad; //In radian


        //Static counter for incrementing unique Ids of ships
        private static int _idStatic = 0;

        //RigidBody 
        private Rigidbody2D _rb;

        private Transform _rudderTransform;

        //We probably don't need this
        private NetworkVariable<Vector2> _mov = new();

        //Environment properties
        //Wind direction vector - this should be a unit vector
        private Vector2 _windDirection;
        //Magnitude of wind vector
        private float _windMagnitude;
        //We probably don't need this
        private const float windAccelerationCoeff = 5000f;
        //Boolean for anchor deployment
        private bool _isAnchorDeployed;

        //Ship properties
        //Boolean for raising or lowering of sail
        private NetworkVariable<bool> _sailLowered = new();


        //Unique ID of ships
        public NetworkVariable<int> Id { private set; get; } = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rudderTransform = _rb.transform.Find("Rudder");
            _windDirection = Vector2.up;
            _windMagnitude = 1f;
            _isAnchorDeployed = false;
            _sailLowered.Value = false;
        }

        //Assign unique Id to ship
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
                    this._rb.AddForce(this.DetermineForce());
                }
                //_rb.angularVelocity += _rudderTorqueCoefficient.Value;
                _rb.velocity /= 1.002f * (_isAnchorDeployed ? 10f : 1f);
                _rb.angularVelocity /= 1.25f;
            }
        }

        private Vector2 DetermineForce() {
            //Rotation of boat as unit vector
             Vector2 shipDirection = new(
                -Mathf.Sin(_rb.rotation * Mathf.Deg2Rad),
                Mathf.Cos(_rb.rotation * Mathf.Deg2Rad)
            );
            //Calculate angle between sailing direction and wind direction
            float sailingAngle =
                Mathf.Acos(Vector2.Dot(shipDirection, _windDirection * -1)) * 180 / Mathf.PI;
                //Only sail when not headwind (i.e. outside No-Go Zone)
            if (sailingAngle > 45f) {
                float newSpeed = _rb.velocity.magnitude + windAccelerationCoeff;
                return shipDirection * _windMagnitude * windAccelerationCoeff;
            }
            return Vector2.zero;
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
            Debug.Log(_maxRudderAngle);
            if (direction && _rudderTransform.rotation.z > -_maxRudderAngle / 2f)
            {
                Debug.Log(_rudderTransform.rotation.z + " " + (_rudderTransform.rotation.z > -_maxRudderAngle));
                _rudderTransform.Rotate(new Vector3(0f, 0f, -1f), Space.Self);
            }
            else if (_rudderTransform.rotation.z < _maxRudderAngle / 2f)
            {
                Debug.Log(_rudderTransform.rotation.z + " " + (_rudderTransform.rotation.z < _maxRudderAngle));
                _rudderTransform.Rotate(new Vector3(0f, 0f, 1f), Space.Self);
            }
        }
    }
}
