using Player.Controlling;
using ScriptableObjects;
using UnityEngine;
using Weapons;
using Vector3 = UnityEngine.Vector3;

namespace Player.Aesthetics
{
    // adjusts camera and viewmodel position
    public class ViewmodelAdjuster : MonoBehaviour
    {
        [SerializeField] private Transform target = null;

        // use scriptable object (le epic overengineering)
        [SerializeField] private ViewmodelHandlingData data = null;

        [SerializeField] private float maxOffset = .1f;

        private Weapon _weapon;
        private NewPlayerController _controller;
        private FootstepManager _footstep;
        private CameraMovement _cameraMovement;

        // lerp values for when goofing around
        private float _sprintLerp;
        private float _airLerp;
        private float _footSideStrengthLerp;
        private float _yVelShiftLerp;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        private void Start()
        {
            _controller = _weapon.Controller;
            _cameraMovement = _controller.CameraMovement;
            _footstep = _controller.FootstepManager;
        }

        private void Update()
        {
            RunLerps();
            
            Vector3 bobOffset = GetBob();
            
            target.localPosition = bobOffset;
            target.transform.position += Vector3.up * _yVelShiftLerp;

            ClampOffset();
        }

        private void ClampOffset()
        {
            if (target.localPosition.sqrMagnitude > Mathf.Pow(maxOffset, 2))
            {
                target.localPosition = target.localPosition.normalized * maxOffset;
            }
        }

        private void RunLerps()
        {
            _sprintLerp = PrintUtil.Damp(_sprintLerp, (_footstep.Sprinting ? 1 : 0), data.SprintLerpLambda,
                Time.deltaTime);
            _airLerp = PrintUtil.Damp(_airLerp, (_controller.Grounded ? 0 : 1), data.AirLerpLambda, Time.deltaTime);
            _footSideStrengthLerp = PrintUtil.Damp(_footSideStrengthLerp, Mathf.Abs(_footstep.CurrentFoot),
                data.FootSideStrengthLambda, Time.deltaTime);
            _yVelShiftLerp = PrintUtil.Damp(_yVelShiftLerp, GetYVelShift(),
                data.YVelShiftLambda, Time.deltaTime);
        }

        private float GetYVelShift()
        {
            return Mathf.Clamp(_controller.RelativeGroundVel.y * data.YVelShiftScale, -maxOffset, maxOffset);
        }

        private Vector3 GetBob()
        {
            Vector3 walkOffset = new Vector3
            (
                data.WalkXMovement.Evaluate(_footstep.StepProgress) * _footstep.CurrentFoot * _footSideStrengthLerp * data.WalkBobScale.x,
                data.WalkYMovement.Evaluate(_footstep.StepProgress) * data.WalkBobScale.y
            );
            Vector3 sprintOffset = new Vector3
            (
                data.SprintXMovement.Evaluate(_footstep.StepProgress) * _footstep.CurrentFoot * _footSideStrengthLerp * data.SprintBobScale.x,
                data.SprintYMovement.Evaluate(_footstep.StepProgress) * data.SprintBobScale.y
            );

            return Vector3.Lerp(walkOffset, sprintOffset, _sprintLerp);
        }
    }
}