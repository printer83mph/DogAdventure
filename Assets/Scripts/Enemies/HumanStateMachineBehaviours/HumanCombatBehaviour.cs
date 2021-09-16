using UnityEngine;

namespace Enemies.HumanStateMachineBehaviours
{
    public class HumanCombatBehaviour : StateMachineBehaviour
    {

        private EnemyMovement _movement;
        private EnemyVision _vision;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _movement = animator.GetComponentInChildren<EnemyMovement>();
            _vision = animator.GetComponentInChildren<EnemyVision>();
            
            // initialize hooks here
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }
    }
}