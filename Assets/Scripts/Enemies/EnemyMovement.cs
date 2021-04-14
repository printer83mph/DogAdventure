using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class EnemyMovement : MonoBehaviour
    {

        private static List<EnemyMovement> enabledMovements = new List<EnemyMovement>();
        
        private const float MaxCornerDistance = .25f;
        private const float NavmeshSampleDistance = 2f;

        [Header("References")]
        [SerializeField] private Rigidbody rb = null;
        [SerializeField] private Transform modelOrientationTransform = null;
        [SerializeField] private Animator animator = null;
        
        [Header("Ground Detection")]
        [SerializeField] private Transform feetTransform = null;
        [SerializeField] private float maxGroundAngle = 45;
        [SerializeField] private float maxGroundDistance = .2f;

        [Header("Pathfinding")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float targetDistance = 1f;
        [SerializeField] private string layerName = "Walkable";

        private bool _grounded;
        private Quaternion _groundRotation;
        
        private bool _locked;
        private Vector3 _target;

        private NavMeshPath _path;
        private Vector3 _partialTargetPosition;
        private bool _atTarget;
        private Vector3 _nextPos;
        private bool _cantPath;

        private int _layerId;

        public Vector3 Velocity => rb.velocity;
        public Vector3 GroundVelocity => _groundRotation * rb.velocity;
        public Vector3 FeetPos => feetTransform.position;

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
        }

        private void OnEnable()
        {
            enabledMovements.Add(this);
        }

        private void OnDisable()
        {
            enabledMovements.Remove(this);
        }

        void Start()
        {
            _path = new NavMeshPath();
            rb.useGravity = false;
            _layerId = NavMesh.AllAreas;
        }

        void Update()
        {
            
            if (!_grounded) return;
            if ((Time.frameCount + enabledMovements.IndexOf(this)) % enabledMovements.Count == 0)
            {
                CalculatePath();
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
            if (_atTarget || _cantPath || _locked)
            {
                AddForceToGoTo(feetTransform.position);
                return;
            }

            // here we're not at our target
            GetNextPos();
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

        private void CheckIfAtTarget()
        {
            _atTarget = Vector3.SqrMagnitude(feetTransform.position - _partialTargetPosition) < Mathf.Pow(targetDistance, 2);
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
                // set our partial target position based
                if (_path.status == NavMeshPathStatus.PathInvalid)
                {
                    
                    // let people know if we can or cant path
                    _cantPath = true;
                }
                else
                {
                    _partialTargetPosition = (_path.status == NavMeshPathStatus.PathPartial
                        ? _path.corners[_path.corners.Length - 2] : _path.corners[_path.corners.Length - 1]);
                    _cantPath = false;
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
                _nextPos = _path.corners[0];
                return;
            }

            // set next pos to the first corner first
            var firstCorner = _path.corners[1];
            
            // check if we're at the first corner already
            if (Vector3.SqrMagnitude(feetTransform.position - firstCorner) < Mathf.Pow(MaxCornerDistance, 2))
            {
                // if so we use the second corner
                _nextPos = _path.corners[2];
                return;
            }
            
            // if we're not then set the first corner to be the next one
            _nextPos = firstCorner;
        }

        private void UpdateAnimatorVelocity()
        {
            Debug.Log(GroundVelocity);
            Vector3 relativeGroundVel = Quaternion.Inverse(modelOrientationTransform.rotation) * GroundVelocity;
            animator.SetFloat("Right", relativeGroundVel.x / moveSpeed);
            animator.SetFloat("Forward", relativeGroundVel.z / moveSpeed);
        }
    }
}