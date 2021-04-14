using System;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controlling
{
    public class PlayerController : MonoBehaviour
    {

        private static PlayerController _main = null;
        public static PlayerController Main => _main;
        
        public delegate void ActionEvent();
        public ActionEvent onJump = delegate {  };
        
        private Rigidbody _rb = null;
        private GroundCheck _groundCheck = null;
        private CameraMovement _cameraMovement = null;
        private PlayerInventory _inventory = null;
        private FootstepManager _footstep = null;

        public Rigidbody Rigidbody => _rb;
        public CameraMovement CameraMovement => _cameraMovement;
        public FootstepManager FootstepManager => _footstep;

        public bool Grounded => _groundCheck.Grounded;
        public Vector3 Velocity => _rb.velocity;

        public Vector3 RelativeGroundVel => _groundCheck.GetRelativeToGroundVelocity(_rb.velocity);
        public Vector2 DeltaAim => _cameraMovement.DeltaAim;

        private PlayerInput _input = null;
        private InputAction m_Move = null;
        private InputAction m_Sprint = null;
        public PlayerInput PlayerInput => _input;
        private Vector2 _movementInput;

        [SerializeField] private Transform orientation = null;
        public Transform Orientation => orientation;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 6f;
        [SerializeField] private float groundAcceleration = 2f;
        [SerializeField] private float airAcceleration = 1f;
        [SerializeField] private float jumpForce = 3f;

        private void Awake()
        {
            _main = this;
            
            _rb = GetComponent<Rigidbody>();
            _groundCheck = GetComponentInChildren<GroundCheck>();
            _cameraMovement = GetComponent<CameraMovement>();
            _inventory = GetComponentInChildren<PlayerInventory>();
            _footstep = GetComponentInChildren<FootstepManager>();
            _input = GetComponent<PlayerInput>();
            
            m_Move = _input.actions["Move"];
            m_Sprint = _input.actions["Sprint"];
        }

        private void OnEnable()
        {
            _input.actions["Jump"].performed += JumpAction;
        }

        private void JumpAction(InputAction.CallbackContext ctx)
        {
            Jump();
        }
        
        private void OnDisable()
        {
            _input.actions["Jump"].performed -= JumpAction;
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
                    Vector3.Project(Physics.gravity, _groundCheck.GroundRotation * Vector3.down),
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
