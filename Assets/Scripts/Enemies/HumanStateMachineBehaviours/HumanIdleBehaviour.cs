using UnityEngine;

namespace Enemies.HumanStateMachineBehaviours
{
    public class HumanIdleBehaviour : StateMachineBehaviour
    {

        // enemy reference
        private HumanEnemy _enemy;

        // logic
        private float _suspicion;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<HumanEnemy>();

            _suspicion = 0;

            // initialize hooks here
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float visionWeight = _enemy.Vision.PlayerVisionWeight;
            _suspicion += (visionWeight > 0
                ? visionWeight * _enemy.SuspicionGainSpeed * Time.deltaTime
                : -_enemy.SuspicionDispelSpeed * Time.deltaTime);
            _suspicion = Mathf.Clamp(_suspicion, 0, 1);
        }
    }
}