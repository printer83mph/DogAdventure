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
        
        private PlayerInventoryWeapon _weapon;
        
        [Header("Overrides")]
        [SerializeField] private FootstepManager footstep;
        [SerializeField] private CameraMovement cameraMovement;

        // lerp values for when goofing around
        private float _sprintLerp;
        private float _airLerp;
        private float _footSideStrengthLerp;
        private float _yVelShiftLerp;
        private Vector2 _lookVelLerp;

        private void Awake()
        {
            _weapon = GetComponent<PlayerInventoryWeapon>();
        }

        private void Start()
        {
            if (!_weapon) return;
            cameraMovement = PlayerController.Main.CameraMovement;
            footstep = PlayerController.Main.FootstepManager;
        }

        private void Update()
        {
            RunLerps();
            
            Vector3 bobOffset = GetBob();

            Vector3 lookVelShift = GetLookVelShift();
            
            target.localPosition = bobOffset;
            target.transform.position += Vector3.up * _yVelShiftLerp;
            target.transform.Translate(lookVelShift);

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
            _sprintLerp = PrintUtil.Damp(_sprintLerp, (footstep.Sprinting ? 1 : 0), data.SprintLerpLambda,
                Time.deltaTime);
            _airLerp = PrintUtil.Damp(_airLerp, (PlayerController.Main.Grounded ? 0 : 1), data.AirLerpLambda, Time.deltaTime);
            _footSideStrengthLerp = PrintUtil.Damp(_footSideStrengthLerp, Mathf.Abs(footstep.CurrentFoot),
                data.FootSideStrengthLambda, Time.deltaTime);
            _yVelShiftLerp = PrintUtil.Damp(_yVelShiftLerp, GetYVelShift(),
                data.YVelShiftLambda, Time.deltaTime);
            _lookVelLerp = PrintUtil.Damp(_lookVelLerp, cameraMovement.DeltaAim / Time.deltaTime, data.LookVelShiftLambda, Time.deltaTime);
        }

        private float GetYVelShift()
        {
            return Mathf.Clamp(PlayerController.Main.RelativeGroundVel.y * data.YVelShiftScale, -maxOffset, maxOffset);
        }

        private Vector3 GetBob()
        {
            Vector3 walkOffset = new Vector3
            (
                data.WalkXMovement.Evaluate(footstep.StepProgress) * footstep.CurrentFoot * _footSideStrengthLerp * data.WalkBobScale.x,
                data.WalkYMovement.Evaluate(footstep.StepProgress) * data.WalkBobScale.y
            );
            Vector3 sprintOffset = new Vector3
            (
                data.SprintXMovement.Evaluate(footstep.StepProgress) * footstep.CurrentFoot * _footSideStrengthLerp * data.SprintBobScale.x,
                data.SprintYMovement.Evaluate(footstep.StepProgress) * data.SprintBobScale.y
            );

            return Vector3.Lerp(walkOffset, sprintOffset, _sprintLerp);
        }

        private Vector3 GetLookVelShift()
        {
            return new Vector3(-_lookVelLerp.x * data.LookVelShiftScale, -_lookVelLerp.y * data.LookVelShiftScale, 0);
        }
    }
}