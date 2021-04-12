using System;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controlling
{
    public class NewPlayerController : MonoBehaviour
    {

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

        private void FixedUpdate()
        {
            Vector2 desiredMovement = m_Move.ReadValue<Vector2>();
            
            RunMovement(desiredMovement);
        }

        private void RunMovement(Vector2 desiredMovement)
        {
            // desired movement direction (without ground angle)
            Vector3 baseEulers = orientation.rotation.eulerAngles;
            Vector3 orientedMovement =  Quaternion.AngleAxis(baseEulers.y, Vector3.up) * new Vector3(desiredMovement.x, 0, desiredMovement.y);
            
            if (_groundCheck.Grounded)
            {
                // sprint/walk speed logic
                orientedMovement *= (m_Sprint.ReadValue<float>() > .5f ? sprintSpeed : walkSpeed);
                
                // add gravity component in ground direction
                _rb.AddForce(
                    _groundCheck.GroundRotation *
                    Vector3.Project(Physics.gravity, _groundCheck.GroundRotation * Vector3.down),
                    ForceMode.Acceleration);

                // if on a rigidbody, shift target movement by its velocity
                if (_groundCheck.GroundRigidbody)
                {
                    Vector3 groundRbVel = Quaternion.Inverse(_groundCheck.GroundRotation) * _groundCheck.GroundRigidbody.GetPointVelocity(_groundCheck.FeetPos);
                    orientedMovement += groundRbVel;
                }
                
                // velocity rotated by inverse ground direction
                Vector3 reorientedVel = Quaternion.Inverse(_groundCheck.GroundRotation) * _rb.velocity;
                Vector2 flatVel = new Vector2(reorientedVel.x, reorientedVel.z);
                
                // move flat vel towards desired movement
                flatVel = Vector2.MoveTowards(flatVel, new Vector2(orientedMovement.x, orientedMovement.z), 
                    groundAcceleration * Time.deltaTime);
                // todo: maybe use AddForce for this?

                reorientedVel.x = flatVel.x;
                reorientedVel.z = flatVel.y;

                _rb.velocity = _groundCheck.GroundRotation * reorientedVel;
            }
            else
            {
                // just add gravity and air acceleration
                _rb.AddForce(Physics.gravity, ForceMode.Acceleration);
                
                // todo: limit this using velocity projected onto desired movement
                float velocityOnDesired = Mathf.Clamp(Vector3.Dot(_rb.velocity, orientedMovement) / sprintSpeed, -1, 1);
                velocityOnDesired = (1 - velocityOnDesired) / 2;
                _rb.AddForce(orientedMovement * (velocityOnDesired * airAcceleration));
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
