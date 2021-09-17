using System;
using Player.Aesthetics;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controlling
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Main { get; private set; } = null;
        public static PlayerInput Input { get; private set; } = null;

        public delegate void ActionEvent();
        public ActionEvent onJump = delegate {  };
        
        private Rigidbody _rb = null;
        private Camera _camera = null;
        private GroundCheck _groundCheck = null;
        private CameraMovement _cameraMovement = null;
        private PlayerInventory _inventory = null;
        private FootstepManager _footstep = null;
        private CameraAdjuster _camAdjuster = null;

        public Rigidbody Rigidbody => _rb;
        public CameraMovement CameraMovement => _cameraMovement;
        public FootstepManager FootstepManager => _footstep;
        public CameraAdjuster CameraAdjuster => _camAdjuster;
        public GroundCheck GroundCheck => _groundCheck;
        
        public bool Grounded => _groundCheck.Grounded;
        public Vector3 Velocity => _rb.velocity;

        public Vector3 RelativeGroundVel => _groundCheck.GetRelativeToGroundVelocity(_rb.velocity);
        public Vector2 DeltaAim => _cameraMovement.DeltaAim;

        private InputAction m_Move = null;
        private InputAction m_Sprint = null;
        private InputAction m_Crouch = null;
        private Vector2 _movementInput;
        private bool _crouched;

        [SerializeField] private Transform orientation = null;
        public Transform Orientation => orientation;
        public Camera Camera => _camera;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 6f;
        [SerializeField] private float groundAcceleration = 2f;
        [SerializeField] private float airAcceleration = 1f;
        [SerializeField] private float jumpForce = 3f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _camera = GetComponentInChildren<Camera>();
            _groundCheck = GetComponentInChildren<GroundCheck>();
            _cameraMovement = GetComponent<CameraMovement>();
            _inventory = GetComponentInChildren<PlayerInventory>();
            _footstep = GetComponentInChildren<FootstepManager>();
            _camAdjuster = GetComponentInChildren<CameraAdjuster>();
            
            Main = this;
            Input = GetComponent<PlayerInput>();

            m_Move = Input.actions["Move"];
            m_Sprint = Input.actions["Sprint"];
            m_Crouch = Input.actions["Crouch"];
        }

        private void OnEnable()
        {
            Input.actions["Jump"].performed += JumpAction;
        }

        private void JumpAction(InputAction.CallbackContext ctx)
        {
            Jump();
        }
        
        private void OnDisable()
        {
            Input.actions["Jump"].performed -= JumpAction;
        }

        private void Start()
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.useGravity = false;
        }

        private void Update()
        {
            _movementInput = m_Move.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            RunMovement(_movementInput);
        }

        private void RunMovement(Vector2 desiredMovement)
        {
            // desired movement direction (without ground angle)
            Vector3 baseEulers = orientation.rotation.eulerAngles;
            Vector3 orientedDesiredMovement =  Quaternion.AngleAxis(baseEulers.y, Vector3.up) * new Vector3(desiredMovement.x, 0, desiredMovement.y);
            
            if (_groundCheck.Grounded)
            {
                // sprint/walk speed logic
                orientedDesiredMovement *= (m_Sprint.ReadValue<float>() > .5f ? sprintSpeed : walkSpeed);

                Vector3 velRelativeToGround = _rb.velocity;
                if (_groundCheck.GroundRigidbody)
                {
                    velRelativeToGround -= _groundCheck.GroundRigidbody.GetPointVelocity(_groundCheck.FeetPos);
                }
                velRelativeToGround = Quaternion.Inverse(_groundCheck.GroundRotation) * velRelativeToGround;

                var velDifference = orientedDesiredMovement - velRelativeToGround;
                var velToAdd = Vector3.ClampMagnitude(velDifference, groundAcceleration * Time.fixedDeltaTime);

                // add gravity component in ground direction
                _rb.AddForce(
                    _groundCheck.GroundRotation *
                    Physics.gravity,
                    ForceMode.Acceleration);

                // add movement force
                _rb.AddForce(velToAdd * _rb.mass, ForceMode.Impulse);
            }
            else
            {
                // just add gravity and air acceleration
                _rb.AddForce(Physics.gravity, ForceMode.Acceleration);
                
                // todo: limit this using velocity projected onto desired movement
                float velocityOnDesired = Mathf.Clamp(Vector3.Dot(_rb.velocity, orientedDesiredMovement) / sprintSpeed, -1, 1);
                velocityOnDesired = (1 - velocityOnDesired) / 2;
                _rb.AddForce(orientedDesiredMovement * (velocityOnDesired * airAcceleration));
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!Grounded) return;
            // Debug.Log("We bouncing");
            // Debug.Log(other.impulse);
            _camAdjuster.AddBounce(- other.impulse.y);
        }

        private void Jump()
        {
            if (!_groundCheck.Grounded) return;
            Debug.Log("jumping");

            onJump();
            
            // todo: make this ground rigidbody dependent
            _rb.velocity += Vector3.down * Mathf.Min(_rb.velocity.y, 0);

            if (_groundCheck.GroundRigidbody)
            {
                // float minYVel = Mathf.Min(_rb.velocity)
                // _rb.velocity += Vector3.down * Mathf.Min(_rb.velocity.y, cth);
                _groundCheck.GroundRigidbody.AddForceAtPosition(Vector3.down * jumpForce, _groundCheck.FeetPos, ForceMode.Impulse);
            }
            
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
