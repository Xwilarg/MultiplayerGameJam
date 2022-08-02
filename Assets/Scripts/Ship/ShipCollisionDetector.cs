using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipCollisionDetector : MonoBehaviour
    {
        private ShipController _controller;

        private void Awake()
        {
            _controller = transform.parent.GetComponent<ShipController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ship")) // Collide with another ship
            {

            }
            else if (collision.CompareTag("Border")) // Collider with the border of the map, we can probably sink the ship
            {

            }
            else if (collision.CompareTag("Player"))
            {
                // Manage case where player go inside boat
            }
            else // Any other item we can roll over
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
