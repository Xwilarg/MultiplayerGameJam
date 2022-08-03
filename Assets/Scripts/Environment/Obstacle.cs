using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerGameJam.Environment
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField]
        internal float _obstacleMass;

        internal Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
    }
}
