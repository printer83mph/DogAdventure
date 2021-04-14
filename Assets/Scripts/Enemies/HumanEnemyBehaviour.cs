using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{

    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Searching,
    }

    public class HumanEnemyBehaviour : MonoBehaviour
    {

        private static List<HumanEnemyBehaviour> _enabledBehaviours = new List<HumanEnemyBehaviour>();
        
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private EnemyVision vision;
        
        [SerializeField] private EnemyState state = EnemyState.Idle;
        
        [SerializeField] private float idleRadius = 2f;
        [SerializeField] private float idleDelayMin = 1f;
        [SerializeField] private float idleDelayMax = 2f;

        private Vector3 _idlePosition;
        private EnemyState _queuedState;

        private void Start()
        {
            _queuedState = state;
            
            // set idle position if we're walking
            if (state == EnemyState.Idle)
            {
                _idlePosition = movement.FeetPos;
            }
            
            StateTransitionTo(EnemyState.Idle, EnemyState.Idle);
        }
        
        private void OnEnable()
        {
            _enabledBehaviours.Add(this);
        }

        private void OnDisable()
        {
            _enabledBehaviours.Remove(this);
        }

        private void Update()
        {
            if (!IsMyTurnToRunLogic()) return;

            // on state change
            if (_queuedState != state)
            {
                StateTransitionTo(_queuedState, state);
                state = _queuedState;
            }
            
            // logic for every tick
            switch (state)
            {
                case EnemyState.Idle:
                    break;
                case EnemyState.Chasing:
                    break;
                case EnemyState.Attacking:
                    break;
                case EnemyState.Searching:
                    break;
            }
        }
        
        private bool IsMyTurnToRunLogic()
        {
            var index = _enabledBehaviours.IndexOf(this);
            if (index == -1) return false;
            return (Time.frameCount + index) % _enabledBehaviours.Count == 0;
        }

        private void StateTransitionTo(EnemyState newState, EnemyState oldState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    StartCoroutine(IdleCoroutine());
                    // check when to switch
                    break;
                case EnemyState.Chasing:
                    break;
                case EnemyState.Attacking:
                    break;
                case EnemyState.Searching:
                    break;
            }
        }

        private IEnumerator IdleCoroutine()
        {
            while (_queuedState == EnemyState.Idle)
            {
                movement.Target = _idlePosition + Quaternion.Euler(0, Random.value * 360, 0) * Vector3.forward * idleRadius;
                yield return new WaitForSeconds(Random.value * (idleDelayMax - idleDelayMin) + idleDelayMin);
            }
        }

    }
}