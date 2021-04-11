using System;
using UnityEngine;

namespace Player.Controlling
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask = default;
        [SerializeField] private float castRadius = .3f;
        [SerializeField] private float reorientationGravity = 2f;

        private bool _grounded = false;
        private Vector3 _groundNormal = default;
        private GameObject _groundObject = null;
        private Rigidbody _groundRigidbody = null;
        private Vector3 _feetPos = default;
        
        public bool Grounded => _grounded;
        public Vector3 GroundNormal => _groundNormal;
        public GameObject GroundObject => _groundObject;
        public Rigidbody GroundRigidbody => _groundRigidbody;
        public Vector3 FeetPos => _feetPos;

        public Vector3 GetRelativeToGround(Vector3 vec) => Quaternion.Inverse(GroundRotation) * vec;

        public Vector3 GetRelativeToGroundVelocity(Vector3 vec) => GetRelativeToGround(_groundRigidbody
            ? vec - _groundRigidbody.GetPointVelocity(_feetPos)
            : vec
        );

        public Quaternion GroundRotation => 
            Quaternion.FromToRotation(Vector3.up, _groundNormal);

        private void FixedUpdate()
        {
            _grounded = false;
            _groundObject = null;
            _groundRigidbody = null;
            _feetPos = transform.position + Vector3.down * castRadius;
            if (Physics.CheckSphere(transform.position, castRadius, groundMask))
            {
                _grounded = true;
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, castRadius * 2, groundMask))
                {
                    _groundNormal = hit.normal;
                    _groundObject = hit.transform.gameObject;
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