using System;
using UnityEngine;

namespace Player.Controlling
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float castRadius = .3f;
        [SerializeField] private float reorientationGravity = 2f;

        private bool _grounded;
        private Vector3 _groundNormal;
        private Rigidbody _groundRigidbody;
        private Vector3 _feetPos;
        
        public bool Grounded => _grounded;
        public Vector3 GroundNormal => _groundNormal;
        public Rigidbody GroundRigidbody => _groundRigidbody;
        public Vector3 FeetPos => _feetPos;

        public Quaternion GroundDirection => 
            Quaternion.FromToRotation(Vector3.up, _groundNormal);

        private void FixedUpdate()
        {
            _grounded = false;
            _groundRigidbody = null;
            _feetPos = transform.position + Vector3.down * castRadius;
            if (Physics.CheckSphere(transform.position, castRadius, groundMask))
            {
                _grounded = true;
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, castRadius * 2, groundMask))
                {
                    _groundNormal = hit.normal;
                    _feetPos = hit.point;
                    if (hit.rigidbody)
                    {
                        _groundRigidbody = hit.rigidbody;
                    }
                }
                else
                {
                    PullBackRotation();
                }
            }
            else
            {
                PullBackRotation();
            }
        }

        private void PullBackRotation()
        {
            _groundNormal = Vector3.MoveTowards(_groundNormal, Vector3.up,
                Time.fixedDeltaTime * reorientationGravity);
        }
    }
}