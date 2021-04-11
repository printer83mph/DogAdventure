using UnityEngine;

namespace Player.Aesthetics
{
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField] private Transform target = null;
        [SerializeField] private float focusSpeed = 0.3f;
        [SerializeField] private float swingSpeed = 15;
        
        private float _focus;
        private Quaternion _rotVel;
        private Quaternion _rot;

        private void Start()
        {
            _focus = 1;
            _rotVel = Quaternion.identity;
            _rot = Quaternion.identity;
        }
        
        private void Update()
        {
            _focus = Mathf.MoveTowards(_focus, 1, Time.deltaTime * focusSpeed);
            _rotVel *=
                // rotates towards inverse rotation scaled by focus
                Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(_rot), _focus * Time.deltaTime);
            _rotVel = Quaternion.Slerp(_rotVel, Quaternion.identity, .05f);
            _rot *= Quaternion.SlerpUnclamped(Quaternion.identity, _rotVel, Time.deltaTime);

            // Debug.Log(_rotVel);
            
            target.localRotation = _rot;
        }

        public void AddImpact(Quaternion rotation)
        {
            _rotVel *= rotation;
            Debug.Log(_rotVel);
            _focus *= 1 - Quaternion.Angle(rotation, Quaternion.identity) / 180;
        }
    }
}