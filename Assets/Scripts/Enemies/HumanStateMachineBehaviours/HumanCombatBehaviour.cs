using Player.Controlling;
using ScriptableObjects.Audio;
using UnityEngine;

namespace Enemies.HumanStateMachineBehaviours
{
    public class HumanCombatBehaviour : StateMachineBehaviour
    {

        private HumanEnemy _enemy;
        private float _suspicion;
        private float _lastSeenPlayer;
        private static readonly int Suspicion = Animator.StringToHash("suspicion");

        private bool TrackingPlayer => Time.time - _lastSeenPlayer < _enemy.Config.PlayerTrackingTime;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            _enemy = animator.GetComponent<HumanEnemy>();
            _suspicion = animator.GetFloat(Suspicion);

            _enemy.Config.AudioChannel.playAudio += OnAudio;

            // initialize hooks here
        }

        private void OnAudio(Vector3 pos, SoundType type, float radius)
        {
            if (Vector3.SqrMagnitude(pos - _enemy.Head.position) > radius * radius) return;

            if (TrackingPlayer) return;

            switch (type)
            {
                case SoundType.Stunning:
                    Debug.Log("Stunned!");
                    break;
                case SoundType.Unimportant:
                    Debug.Log("Unimportant sound bro");
                    break;
                default:
                    _enemy.lastKnownPosition = pos;
                    break;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            _enemy.Config.AudioChannel.playAudio -= OnAudio;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (_enemy.Vision.CanSee(PlayerController.Main.Camera.transform.position, .2f))
            {
                float visionWeight = _enemy.Vision.PlayerVisionWeight;

                if (visionWeight > _enemy.Config.CombatVisionWeightThreshold)
                {
                    // we "see" the player
                    _enemy.lastKnownPosition = PlayerController.Main.transform.position;
                    _lastSeenPlayer = Time.time;
                }
                
                float toAdd = (visionWeight > 0
                    ? visionWeight * _enemy.Config.SusGainSpeed
                    : -_enemy.Config.SusDispelSpeed);

                _suspicion = Mathf.Clamp(_suspicion + toAdd * Time.deltaTime, 0, 1);
                animator.SetFloat(Suspicion, _suspicion);
            }
            
            _enemy.Movement.Target = _enemy.lastKnownPosition;
            
            // TODO: use current weapon data to figure out prime attack range
            _enemy.Movement.locked = false;

            // TODO: if we aren't "tracking player" then do look around animation
        }
    }
}