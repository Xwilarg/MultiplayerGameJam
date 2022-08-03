using UnityEngine;
using MultiplayerGameJam.Environment;

namespace MultiplayerGameJam.Ship
{
    public class ShipCollisionDetector : MonoBehaviour
    {
        private ShipController _controller;

        private const float _restitutionCoefficient = 0.8f;

        private void Awake()
        {
            _controller = transform.parent.GetComponent<ShipController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ship")) // Collide with another ship
            { }
            else if (collision.CompareTag("Border")) // Collider with the border of the map, we can probably sink the ship
            { }
            else if (collision.CompareTag("Player"))
            {
                // Manage case where player go inside boat
            }
            else // Obstacles
            {
                Obstacle collidingObstacle = collision.GetComponent<Obstacle>();
                this._controller.setVelocityServerRpc(
                    (
                        _restitutionCoefficient
                            * collidingObstacle._obstacleMass
                            * (-_controller._rb.velocity)
                        + _controller._shipMass * _controller._rb.velocity
                        + collidingObstacle._obstacleMass * collidingObstacle._rb.velocity
                    ) / (_controller._shipMass + collidingObstacle._obstacleMass)
                );
                this._controller._rb.angularVelocity = -this._controller._rb.angularVelocity / 1.5f;
            }
        }
    }
}
