using UnityEngine;

namespace MultiplayerGameJam.Player
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform _target;
        private Vector2 _offset;

        public Transform Target
        {
            set
            {
                _target = value;
                _offset = transform.position - Target.position;
            }
            private get => _target;
        }

        private void Update()
        {
            if (_target != null)
            {
                var p = _offset + (Vector2)Target.position;
                transform.position = new Vector3(p.x, p.y, transform.position.z);
            }
        }
    }
}
