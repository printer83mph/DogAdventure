using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controlling
{
    public class CameraMovement : MonoBehaviour
    {
        private Rigidbody _rb;
        
        [SerializeField] private Transform orientation;

        [SerializeField] private PlayerInput input;
        private InputAction m_Aim;

        [SerializeField] private float sensitivity = .01f;
        [SerializeField] private float maxVerticalLook = 90f;

        private float _lookHorizontal;
        private float _lookVertical;
        
        private Vector2 _deltaAim;
        public Vector2 DeltaAim => _deltaAim;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            m_Aim = input.actions["Aim"];
        }
        
        private void Update()
        {
            Vector2 mouseAim = m_Aim.ReadValue<Vector2>() * sensitivity;

            float newLookHorizontal = _lookHorizontal + mouseAim.x;
            float newLookVertical = Mathf.Clamp(_lookVertical - mouseAim.y, -maxVerticalLook, maxVerticalLook);

            _deltaAim.Set(newLookHorizontal - _lookHorizontal, newLookVertical - _lookVertical);

            _lookHorizontal = newLookHorizontal % 360;
            _lookVertical = newLookVertical;
            
            orientation.rotation = Quaternion.Euler(_lookVertical, _lookHorizontal, 0);
        }
    }
}