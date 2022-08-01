using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipCollisionDetector : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            transform.parent.GetComponent<ShipController>();
        }
    }
}
