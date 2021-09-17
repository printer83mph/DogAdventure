using System;
using ScriptableObjects.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.HumanStateMachineBehaviours
{
    public class HumanIdleBehaviour : StateMachineBehaviour
    {

        private static float CheckingSoundVisionWeightThreshold = .2f;

        // animator static reference stuff
        private static readonly int Suspicion = Animator.StringToHash("suspicion");

        // enemy reference
        private HumanEnemy _enemy;
        private float _suspicion;

        // private state things
        private bool _checkingOutSound;
        private float _positionChangeDelay;
        private float _lastPositionChange;
        private Vector3 _originalPosition;

        private void UpdateIdleTiming()
        {
            _positionChangeDelay =
                Random.Range(_enemy.Config.IdleDelayMin, _enemy.Config.IdleDelayMax);
            _lastPositionChange = Time.time;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.GetComponent<HumanEnemy>();
            _suspicion = animator.GetFloat(Suspicion);
            
            UpdateIdleTiming();
            if (_originalPosition == Vector3.zero) _originalPosition = _enemy.Feet.position;
            _enemy.Movement.turnTowardsMoveDirection = true;

            _enemy.Config.AudioChannel.playAudio += OnAudio;
            Debug.Log("you are been trolled");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.Config.AudioChannel.playAudio -= OnAudio;
        }

        private void OnAudio(Vector3 pos, SoundType type, float radius)
        {
            if (!(Vector3.SqrMagnitude(pos - _enemy.Head.position) < radius * radius)) return;
            // we heard the sound
            switch (type)
            {
                case SoundType.Suspicious:
                    _enemy.lastKnownPosition = pos;
                    _enemy.Movement.Target = pos;
                    _checkingOutSound = true;
                    break;
                case SoundType.Alarming:
                    _enemy.lastKnownPosition = pos;
                    _enemy.Movement.Target = pos;
                    _checkingOutSound = true;
                    Debug.Log("Heard alarming sound Bro!");
                    break;
                case SoundType.Stunning:
                    break;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // figure out vision weight addition
            float visionWeight = _enemy.Vision.PlayerVisionWeight;
            float toAdd = (visionWeight > 0
                ? visionWeight * _enemy.Config.SusGainSpeed
                : -_enemy.Config.SusDispelSpeed);

            // add to private suspicion value and update state machine's suspicion
            _suspicion = Mathf.Clamp(_suspicion + toAdd * Time.deltaTime, 0, 1);
            animator.SetFloat(Suspicion, _suspicion);

            UpdateMovement();
        }

        // if suspicious, move towards suspicious sound
        private void UpdateMovement()
        {
            if (_checkingOutSound)
            {
                bool canSeeSpot = _enemy.Vision.VisionWeight(_enemy.lastKnownPosition) > CheckingSoundVisionWeightThreshold;
                _enemy.Movement.locked = canSeeSpot;
            }
            else
            {
                _enemy.Movement.locked = false;
                
                if (!(Time.time - _lastPositionChange > _positionChangeDelay)) return;
                // setting a new idle position
                UpdateIdleTiming();
                Vector2 newPos = Random.insideUnitCircle * _enemy.Config.IdleRadius;
                _enemy.Movement.Target = _originalPosition + new Vector3(newPos.x, 0, newPos.y);
            }
        }
    }
}