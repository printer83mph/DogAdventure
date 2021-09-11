using Player.Controlling;
using Unity.Mathematics;
using UnityEngine;

namespace Player.Aesthetics
{
    // manages target's local position and rotation based on shake/kick inputs
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField] private Transform target = null;
        [SerializeField] private float gravity = 1;
        [SerializeField] private float damping = .9f;

        private Vector2 _rotation = Vector2.zero;
        private Vector2 _velocity = Vector2.zero;

        private void Update()
        {
            // add gravity
            _velocity += -_rotation * Mathf.Min(gravity * Time.deltaTime, 1);
            
            // add damping
            _velocity = _velocity * Mathf.Min(Mathf.Pow(damping, Time.deltaTime), 1);

            _rotation += _velocity * Mathf.Min(Time.deltaTime, 1);
            
            // visual update
            target.localRotation = Quaternion.Euler(-_rotation.y, _rotation.x, 0);
        }

        public void AddKick(Vector2 rotation)
        {
            _velocity += rotation;
        }
    }
}
