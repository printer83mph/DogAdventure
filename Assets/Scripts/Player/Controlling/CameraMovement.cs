using Player.Aesthetics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controlling
{
    public class CameraMovement : MonoBehaviour
    {
        private Rigidbody _rb;
        // private CameraAdjuster _camAdjuster;
        // public CameraAdjuster CameraAdjuster => _camAdjuster;
        
        [SerializeField] private Transform orientation = null;

        [SerializeField] private PlayerInput input = null;
        private InputAction m_Aim = null;

        [SerializeField] private float sensitivity = .01f;
        [SerializeField] private float maxVerticalLook = 90f;

        private float _lookHorizontal = 0;
        private float _lookVertical = 0;
        
        private Vector2 _deltaAim = default;
        public Vector2 DeltaAim => _deltaAim;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            // _camAdjuster = GetComponentInChildren<CameraAdjuster>();
            m_Aim = input.actions["Aim"];
        }
        
        private void Update()
        {
            Vector2 mouseAim = m_Aim.ReadValue<Vector2>() * sensitivity;

            float newLookHorizontal = _lookHorizontal + mouseAim.x;
            float newLookVertical = Mathf.Clamp(_lookVertical - mouseAim.y, -maxVerticalLook, maxVerticalLook);

            _deltaAim.Set(newLookHorizontal - _lookHorizontal, -(newLookVertical - _lookVertical));

            _lookHorizontal = newLookHorizontal % 360;
            _lookVertical = newLookVertical;
            
            orientation.rotation = Quaternion.Euler(_lookVertical, _lookHorizontal, 0);
        }
    }
}