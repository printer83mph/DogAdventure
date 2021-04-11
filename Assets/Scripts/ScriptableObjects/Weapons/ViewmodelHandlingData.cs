using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ViewmodelHandlingData", menuName = "ScriptableObjects/Weapons/ViewmodelHandlingData", order = 0)]
    public class ViewmodelHandlingData : ScriptableObject
    {
        [Header("Bob")]
        [SerializeField] private Vector2 walkBobScale = default;
        [SerializeField] private Vector2 sprintBobScale = default;
        [SerializeField] private AnimationCurve walkYMovement = null;
        [SerializeField] private AnimationCurve walkXMovement = null;
        [SerializeField] private AnimationCurve sprintYMovement = null;
        [SerializeField] private AnimationCurve sprintXMovement = null;
        
        [Header("General")]
        [SerializeField] private float sprintLerpLambda = 5;
        [SerializeField] private float airLerpLambda = 3;
        [SerializeField] private float footSideStrengthLambda = 5;

        [Header("Sway")]
        [SerializeField] private float yVelShiftLambda = 7;
        [SerializeField] private float yVelShiftScale = -.01f;

        public Vector2 WalkBobScale => walkBobScale;
        public Vector2 SprintBobScale => sprintBobScale;
        public AnimationCurve WalkYMovement => walkYMovement;
        public AnimationCurve WalkXMovement => walkXMovement;
        public AnimationCurve SprintYMovement => sprintYMovement;
        public AnimationCurve SprintXMovement => sprintXMovement;
        
        public float SprintLerpLambda => sprintLerpLambda;
        public float AirLerpLambda => airLerpLambda;
        public float FootSideStrengthLambda => footSideStrengthLambda;
        
        public float YVelShiftLambda => yVelShiftLambda;
        public float YVelShiftScale => yVelShiftScale;
    }
}