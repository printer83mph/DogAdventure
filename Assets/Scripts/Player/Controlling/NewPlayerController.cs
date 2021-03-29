using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controlling
{
    public class NewPlayerController : MonoBehaviour
    {
        private Rigidbody _rb;
        private GroundCheck _groundCheck;

        private CameraMovement _cameraMovement;
        public Vector2 DeltaAim => _cameraMovement.DeltaAim;

        [SerializeField] private PlayerInput input;
        private InputAction m_Move;
        private InputAction m_Sprint;

        [SerializeField] private Transform orientation;

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
            
            m_Move = input.actions["Move"];
            m_Sprint = input.actions["Sprint"];
        }

        private void OnEnable()
        {
            input.actions["Jump"].performed += JumpAction;
        }

        private void JumpAction(InputAction.CallbackContext ctx)
        {
            Jump();
        }
        
        private void OnDisable()
        {
            input.actions["Jump"].performed -= JumpAction;
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
            // desired movement direction
            Vector3 baseEulers = orientation.rotation.eulerAngles;
            Vector3 orientedMovement =  Quaternion.AngleAxis(baseEulers.y, Vector3.up) * new Vector3(desiredMovement.x, 0, desiredMovement.y);
            
            if (_groundCheck.Grounded)
            {
                _rb.AddForce(
                    _groundCheck.GroundDirection *
                    Vector3.Project(Physics.gravity, _groundCheck.GroundDirection * Vector3.down),
                    ForceMode.Acceleration);
                
                orientedMovement *= (m_Sprint.ReadValue<float>() > .5f ? sprintSpeed : walkSpeed);

                if (_groundCheck.GroundRigidbody)
                {
                    Vector3 groundRbVel = Quaternion.Inverse(_groundCheck.GroundDirection) * _groundCheck.GroundRigidbody.GetPointVelocity(_groundCheck.FeetPos);
                    orientedMovement += groundRbVel;
                }
                
                Vector3 reorientedVel = Quaternion.Inverse(_groundCheck.GroundDirection) * _rb.velocity;

                Vector2 flatVel = new Vector2(reorientedVel.x, reorientedVel.z);
                flatVel = Vector2.MoveTowards(flatVel, new Vector2(orientedMovement.x, orientedMovement.z), 
                    groundAcceleration * Time.deltaTime);
                // todo: maybe use AddForce for this?
                
                // reorientedVel.x = Mathf.MoveTowards(reorientedVel.x, orientedMovement.x,
                //     groundAcceleration * Time.deltaTime);
                // reorientedVel.z = Mathf.MoveTowards(reorientedVel.z, orientedMovement.z,
                //     groundAcceleration * Time.deltaTime);

                reorientedVel.x = flatVel.x;
                reorientedVel.z = flatVel.y;

                _rb.velocity = _groundCheck.GroundDirection * reorientedVel;
                // _rb.velocity = orientedMovement * walkSpeed;
            }
            else
            {
                _rb.AddForce(Physics.gravity, ForceMode.Acceleration);
                _rb.AddForce(orientedMovement * airAcceleration);
            }
        }

        private void Jump()
        {
            if (!_groundCheck.Grounded) return;
            Debug.Log("jumping");
            
            // todo: make this ground rigidbody dependent
            _rb.velocity += Vector3.down * Mathf.Min(_rb.velocity.y);

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
