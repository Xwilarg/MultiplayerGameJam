using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _mov = new();

        private Vector2 _wind;

        private bool _sailLowered;

        private const float _oceanFrictionMagnitude = 0.1f;

        private const float shipAccelerationCoeff = 0.3f;


        private const float _maxVelocity = 5f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _wind = Vector2.down;
            _sailLowered = false;
        }

        private void FixedUpdate()
        {
            //Skip all logic if velocity is zero
            if (_rb.velocity.SqrMagnitude() > 0)
            {
                if (_sailLowered)
                {
                    accelerateByWind();
                }

                //Ship slowed down by friction from ocean water
                oceanFrictionVelocityDecrease();
            }
        }

        [ServerRpc]
        public void AddRelativeVelocityServerRpc(Vector2 pos)
        {
            _rb.velocity = (pos.y * transform.up + pos.x * transform.right).normalized * 10f;
        }

        [ServerRpc]
        public void AddTorqueServerRpc(float torque)
        {
            _rb.AddTorque(torque);
        }

        public void toggleSail()
        {
            _sailLowered = !_sailLowered;
        }

        private void accelerateByWind()
        {
            //ToDo: Finish adding acceleration determined by wind direction i.e. headwind calculation
        }

        private void oceanFrictionVelocityDecrease()
        {
            _rb.velocity =
                _rb.velocity.normalized * (_rb.velocity.magnitude - _oceanFrictionMagnitude);
            if (_rb.velocity.sqrMagnitude < 0)
            {
                _rb.velocity = Vector2.zero;
            }
        }
    }
}
