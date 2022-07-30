using UnityEngine;

namespace MultiplayerGameJam.Ship
{
    public class ShipManager : MonoBehaviour
    {
        public static ShipManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private GameObject _referenceShip;

        public Transform ShipParent => _referenceShip.transform;
    }
}
