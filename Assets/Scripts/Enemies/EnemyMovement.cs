using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

namespace Enemies
{
    public class EnemyMovement : MonoBehaviour
    {

        private static List<EnemyMovement> enabledMovements = new List<EnemyMovement>();
        
        private const float MaxCornerDistance = .25f;
        private const float NavmeshSampleDistance = 2f;

        [Header("References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform modelOrientationTransform;
        [SerializeField] private Collider collider;
        [SerializeField] private Animator animator;
        
        [Header("Ground Detection")]
        [SerializeField] private Transform feetTransform = null;
        [SerializeField] private float maxGroundAngle = 45;
        [SerializeField] private float maxGroundDistance = .2f;

        [Header("Pathfinding")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float targetDistance = 1f;
        [SerializeField] private string layerName = "Walkable";
        
        [Header("Movement")]
        public bool locked;
        public bool turnTowardsMoveDirection;
        [SerializeField] private float turnSpeed;
        
        [Header("Head")]
        [SerializeField] private MultiAimConstraint headConstraint;
        [SerializeField] private Transform headTarget;
        public bool lockHeadTargetWorldSpace;

        // ground logic
        private bool _grounded;
        private Quaternion _groundRotation;

        // pathfinding
        private Vector3 _target;
        private Vector3 _headTargetWorldPos;
        private NavMeshPath _path;
        private Vector3 _partialTargetPosition;
        public bool AtTarget { get; private set; }
        private Vector3 _nextPos;
        private bool _cantPath;

        private int _layerId;

        // for other scripts
        public Vector3 Velocity => rb.velocity;
        public Vector3 GroundVelocity => _groundRotation * rb.velocity;
        public Vector3 FeetPos => feetTransform.position;
        public MultiAimConstraint HeadConstraint => headConstraint;

        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = value;
                if (_grounded) CalculatePath();
            }
        }

        private void Awake()
        {
            Target = feetTransform.position;
            _layerId = NavMesh.AllAreas;
        }

        private void OnEnable()
        {
            enabledMovements.Add(this);
            rb.isKinematic = false;
            collider.enabled = true;
        }

        private void OnDisable()
        {
            enabledMovements.Remove(this);
            rb.isKinematic = true;
            collider.enabled = false;
            
            animator.SetFloat("Forward", 0);
            animator.SetFloat("Right", 0);
        }

        void Start()
        {
            _path = new NavMeshPath();
            rb.useGravity = false;
            headTarget.position =
                headConstraint.data.constrainedObject.position + transform.TransformVector(0, 0, 1);
        }

        void Update()
        {
            
            // do head logic
            // if (lockHeadTargetWorldSpace) headTarget.position = _headTargetWorldPos;
            // else _headTargetWorldPos = headTarget.position;

            if (!_grounded) return;
            if (AtTarget) return;
            if ((Time.frameCount + enabledMovements.IndexOf(this)) % enabledMovements.Count == 0)
            {
                CalculatePath();
            }

            if (turnTowardsMoveDirection)
            {
                Vector3 vecToTarget = _nextPos - feetTransform.position;
                vecToTarget.y = 0;
                modelOrientationTransform.rotation = Quaternion.RotateTowards(modelOrientationTransform.rotation,
                    Quaternion.FromToRotation(Vector3.forward, vecToTarget),
                    Time.deltaTime * turnSpeed);
            }
        }

        void FixedUpdate()
        {
            
            UpdateAnimatorVelocity();
            GetGrounded();

            if (_grounded) MovementLogic();
            
            // add gravity force
            rb.AddForce(_groundRotation * Physics.gravity, ForceMode.Acceleration);

        }

        private void MovementLogic()
        {
            CheckIfAtTarget();

            // if we're already there we don't care
            if (AtTarget || _cantPath || locked)
            {
                AddForceToGoTo(feetTransform.position);
                return;
            }

            // here we're not at our target
            if (AtNextPos()) GetNextPos();
            AddForceToGoTo(_nextPos);
            
        }

        private void AddForceToGoTo(Vector3 position)
        {
            var inverseRotation = Quaternion.Inverse(_groundRotation);
            // get desired velocity
            var desiredDirection = _groundRotation * (position - feetTransform.position);
            desiredDirection.y = 0;
            // normalize and scale
            desiredDirection = desiredDirection.normalized * moveSpeed;
            Debug.DrawRay(feetTransform.position, desiredDirection * 2, Color.blue, .1f);

            var orientedVel = inverseRotation * rb.velocity;
            orientedVel.y = 0;
            var velDifference = desiredDirection - orientedVel;

            var velToAdd = Vector3.ClampMagnitude(velDifference, acceleration * Time.fixedDeltaTime);
            Debug.DrawRay(feetTransform.position, velToAdd);
            velToAdd = _groundRotation * velToAdd;

            rb.AddForce(velToAdd * rb.mass, ForceMode.Impulse);
        }

        // if we're at our actual target
        private void CheckIfAtTarget()
        {
            AtTarget = Vector3.SqrMagnitude(feetTransform.position - _partialTargetPosition) < Mathf.Pow(targetDistance, 2);
        }

        // if we're at the next queued position
        private bool AtNextPos()
        {
            return Vector3.SqrMagnitude(feetTransform.position - _nextPos) < Mathf.Pow(targetDistance, 2);
        }

        private void GetGrounded()
        {
            Ray ray = new Ray(feetTransform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, maxGroundDistance)
                && Vector3.Angle(Vector3.up, hit.normal) < maxGroundAngle)
            {
                if (!_grounded) OnTouchGround();
                _grounded = true;
                _groundRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
            else
            {
                if (_grounded) OnLeaveGround();
                _grounded = false;
                _groundRotation = Quaternion.identity;
            }
        }

        private void OnTouchGround()
        {
            Debug.Log("We touched the ground");
            animator.SetBool("Grounded", true);
        }
        
        private void OnLeaveGround()
        {
            Debug.Log("We left the ground");
            animator.SetBool("Grounded", false);
        }

        private void CalculatePath()
        {

            // if we can even pathfind
            if (NavMesh.SamplePosition(feetTransform.position, out NavMeshHit hit, NavmeshSampleDistance, _layerId))
            {
                NavMesh.CalculatePath(hit.position, _target, _layerId, _path);
                if (_path.status == NavMeshPathStatus.PathInvalid)
                {
                    
                    // let people know if we can or cant path
                    _cantPath = true;
                }
                else
                {
                    _cantPath = false;
                    
                    // set our partial target position based
                    if (_path.corners.Length == 1)
                    {
                        _partialTargetPosition = _path.corners[0];
                        _nextPos = _path.corners[0];
                        return;
                    }
                    _partialTargetPosition = (_path.status == NavMeshPathStatus.PathPartial
                        ? _path.corners[_path.corners.Length - 2] : _path.corners[_path.corners.Length - 1]);
                    _nextPos = _path.corners[1];
                }
            }
        }

        // run all the time when we're pathing. finds the next
        private void GetNextPos()
        {

            if (_path.corners.Length == 0)
            {
                Debug.Log("No corners..?");
                _nextPos = _target;
                return;
            }
            if (_path.corners.Length == 1)
            {
                Debug.Log("Only one corner?");
                _nextPos = _path.corners[0];
                return;
            }

            for (var i = _path.corners.Length - 1; i > 0; i--)
            {
                if (Vector3.SqrMagnitude(feetTransform.position - _path.corners[i]) < Mathf.Pow(MaxCornerDistance, 2))
                {
                    _nextPos = _path.corners[i + 1];
                    return;
                }
            }

        }

        private void UpdateAnimatorVelocity()
        {
            Vector3 relativeGroundVel = Quaternion.Inverse(modelOrientationTransform.rotation) * GroundVelocity;
            animator.SetFloat("Right", relativeGroundVel.x / moveSpeed);
            animator.SetFloat("Forward", relativeGroundVel.z / moveSpeed);
        }
    }
}
