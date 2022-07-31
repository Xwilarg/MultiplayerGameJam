﻿using Unity.Netcode;
using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipController : NetworkBehaviour
    {
        private Rigidbody2D _rb;

        private NetworkVariable<Vector2> _mov = new();

        //Environment properties
        private Vector2 _windDirection;
        private float _windMagnitude;
        private const float windAccelerationCoeff = 0.1f;
        
        //Ship properties
        private NetworkVariable<bool> _sailLowered = new();
        private const float _maxShipVelocity = 3f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _windDirection = Vector2.up;
            _windMagnitude = 1f;
            _sailLowered.Value = false;
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                _rb.velocity /= 1.1f;
                _rb.angularVelocity /= 1.1f;

                if (_sailLowered.Value)
                {
                    accelerateBySailServerRpc();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)] // TODO: Bad idea
        public void AddRelativeVelocityServerRpc(Vector2 pos)
        {
            _rb.velocity = (pos.y * transform.up + pos.x * transform.right).normalized * 10f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddTorqueServerRpc(float torque)
        {
            _rb.angularVelocity = torque;
        }

        //Raise or lower sail
        [ServerRpc]
        public void ToggleSailServerRpc()
        {
            _sailLowered.Value = !(_sailLowered.Value);
        }

        [ServerRpc(RequireOwnership = false)]
        private void accelerateBySailServerRpc()
        {
            Vector2 shipDirection = new Vector2(
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
    }
}
