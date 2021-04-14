using System;
using UnityEngine;

namespace Player.Controlling
{
    public class FootstepManager : MonoBehaviour
    {
        public delegate void FootstepEvent(int newFoot, bool loud);
        public FootstepEvent onFootstep = delegate {  };
        
        [SerializeField] private PlayerController controller = null;

        [Header("Base Movement")]
        [SerializeField] private float stepLength = 2.5f;
        [SerializeField] private float strideLength = 4;
        [SerializeField] private float stepVelocityThreshold = .1f;
        [SerializeField] private float runVelocityThreshold = 5;

        [Header("Lerping and such")]
        [SerializeField] private float cancelStepLambda = 8f;
        [SerializeField] private float midAirStepLambda = 5f;

        private int _currentFoot = 0;
        private float _stepProgress = 0;
        private bool _sprinting = false;
        private bool _midAir = false;

        public int CurrentFoot => _currentFoot;
        public float StepProgress => _stepProgress;
        public bool Sprinting => _sprinting;

        private void OnEnable()
        {
            controller.onJump += OnJump;
        }

        private void OnDisable()
        {
            controller.onJump -= OnJump;
        }

        private void OnJump()
        {
            // todo: fix double footstep when jumping from standstill
            onFootstep(_currentFoot, _sprinting);
        }

        private void Update()
        {
            if (controller.Grounded)
            {
                if (_midAir)
                {
                    // on landing.. maybe?
                    // _stepProgress = 1;
                    onFootstep(_currentFoot, _sprinting);
                }

                _midAir = false;
                var vel = controller.RelativeGroundVel.magnitude;
                if (vel > stepVelocityThreshold)
                {
                    _sprinting = vel > runVelocityThreshold;
                    if (_currentFoot == 0) _currentFoot = -1;
                    // walking
                    _stepProgress += vel / (_sprinting ? strideLength : stepLength) * Time.deltaTime;
                    if (_stepProgress >= 1)
                    {
                        _currentFoot = -_currentFoot;
                        onFootstep(_currentFoot, _sprinting);
                        _stepProgress = 0;
                    }
                }
                else
                {
                    // standing "still"
                    _sprinting = false;
                    // moving to standstill
                    _stepProgress = PrintUtil.Damp(_stepProgress, Mathf.Round(_stepProgress), cancelStepLambda, Time.deltaTime);
                    if (_stepProgress <= .01)
                    {
                        _stepProgress = 1;
                        _currentFoot = -_currentFoot;
                    }
                    // _currentFoot = 0;
                }
            }
            else
            {
                // in air
                _sprinting = false;
                // _stepProgress = 0;
                // _currentFoot = 0;
                _stepProgress = PrintUtil.Damp(_stepProgress, Mathf.Round(_stepProgress), midAirStepLambda, Time.deltaTime);
                if (_stepProgress >= .99)
                {
                    _stepProgress = 0;
                    _currentFoot = -_currentFoot;
                }
                _midAir = true;
            }
        }
    }
}