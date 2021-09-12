using System;
using Player.Controlling;
using Unity.Mathematics;
using UnityEngine;

namespace Player.Aesthetics
{
    // manages target's local position and rotation based on shake/kick inputs
    public class CameraAdjuster : MonoBehaviour
    {
        [Header("Kick Rotation")]
        [SerializeField] private Transform target = null;
        [SerializeField] private float gravity = 1;
        [SerializeField] private float damping = .9f;

        private Vector2 _kickRotation = Vector2.zero;
        private Vector2 _velocity = Vector2.zero;

        [Header("Head Shift")]
        [SerializeField] private float shiftScale = .1f;
        [SerializeField] private float shiftLambda = 12f;

        [Header("View Bounce")]
        [SerializeField] private AnimationCurve viewBounceCurve = null;
        [SerializeField] private float viewBounceScale = .001f;
        [SerializeField] private float viewBounceSpeedScale = 2;
        private float _lastViewBounce = 0;
        private float _viewBounceAmplitude = .1f;
        private float _viewBounceSpeed = 2f;

        private Vector3 _shiftPosition = Vector3.zero;
        
        // misc math stuff

        private Vector3 _basePos;

        private void Awake()
        {
            _basePos = target.localPosition;
        }

        // view kick
        
        private void UpdateKick()
        {
            // add gravity
            _velocity += -_kickRotation * Mathf.Min(gravity * Time.deltaTime, 1);
            
            // add damping
            _velocity = _velocity * Mathf.Min(Mathf.Pow(damping, Time.deltaTime), 1);

            _kickRotation += _velocity * Time.deltaTime;
        }
        
        public void AddKick(Vector2 rotation)
        {
            _velocity += rotation;
        }

        // view bounce
        
        private float GetViewBounce()
        {
            return viewBounceCurve.Evaluate((Time.time - _lastViewBounce) * _viewBounceSpeed) * _viewBounceAmplitude;
        }

        public void AddBounce(float strength)
        {
            if (strength > -1f) return;
            _lastViewBounce = Time.time;
            _viewBounceAmplitude = strength * viewBounceScale;
            Debug.Log(_viewBounceAmplitude);
            _viewBounceSpeed = viewBounceSpeedScale / Mathf.Pow(Mathf.Abs(_viewBounceAmplitude), .5f);
        }

        // head shift
        
        private void UpdateShift()
        {
            var clampedVel = Vector3.ProjectOnPlane(PlayerController.Main.Velocity, PlayerController.Main.GroundCheck.GroundNormal);
            _shiftPosition = PrintUtil.Damp(_shiftPosition, PlayerController.Main.Grounded ? clampedVel * shiftScale : Vector3.zero, shiftLambda, Time.deltaTime);
        }
        
        private void Update()
        {
            UpdateKick();
            UpdateShift();
            
            float bounce = GetViewBounce();
            
            // update actual position and rotation
            target.localRotation = Quaternion.Euler(-_kickRotation.y, _kickRotation.x, 0);
            
            target.localPosition = _basePos;
            target.position += _shiftPosition + Vector3.down * bounce;
        }
    }
}
