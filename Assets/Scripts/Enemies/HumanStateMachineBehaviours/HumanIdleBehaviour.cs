using ScriptableObjects.Audio;
using UnityEngine;

namespace Enemies.HumanStateMachineBehaviours
{
    public class HumanIdleBehaviour : StateMachineBehaviour
    {

        // animator static reference stuff
        private static readonly int Suspicion = Animator.StringToHash("suspicion");

        // enemy reference
        private HumanEnemy _enemy;
        private float _suspicion;
        
        // private state things
        private bool _checkingOutSound;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<HumanEnemy>();
            // _suspicion = animator.GetFloat(Suspicion);

            _enemy.AudioChannel.playAudio += OnAudio;
            Debug.Log("you are been trolled");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.AudioChannel.playAudio -= OnAudio;
        }

        private void OnAudio(Vector3 pos, SoundType type, float radius)
        {
            Debug.Log("Heard audio bro!");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // figure out vision weight addition
            float visionWeight = _enemy.Vision.PlayerVisionWeight;
            float toAdd = (visionWeight > 0
                ? visionWeight * _enemy.SuspicionGainSpeed * Time.deltaTime
                : -_enemy.SuspicionDispelSpeed * Time.deltaTime);
            
            // add to private suspicion value and update state machine's suspicion
            _suspicion = Mathf.Clamp(_suspicion + toAdd, 0, 1);
            // animator.SetFloat(Suspicion, _suspicion);
        }
    }
}