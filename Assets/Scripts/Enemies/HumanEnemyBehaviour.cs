using System;
using System.Collections;
using System.Collections.Generic;
using Stims;
using UnityEngine;
using UnityEngine.Events;
using World;
using World.StimListeners;
using Random = UnityEngine.Random;

namespace Enemies
{

    public enum EnemyState
    {
        Idle,
        Chasing,
        Suspicious,
        Attacking,
        Searching,
    }

    public class HumanEnemyBehaviour : MonoBehaviour
    {

        private static readonly List<HumanEnemyBehaviour> EnabledBehaviours = new List<HumanEnemyBehaviour>();
        
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private Health health;
        [SerializeField] private EnemyVision vision;
        [SerializeField] private Animator animator;
        [SerializeField] private RagdollController ragdoll;
        
        [SerializeField] private EnemyState state = EnemyState.Idle;
        
        [SerializeField] private float idleRadius = 2f;
        [SerializeField] private float idleDelayMin = 1f;
        [SerializeField] private float idleDelayMax = 2f;

        private Vector3 _idlePosition;

        private void OnEnable()
        {
            // set idle position if we're walking
            if (state == EnemyState.Idle)
            {
                _idlePosition = movement.FeetPos;
            }
            
            // todo: will we ever actually use enabled behaviours...?
            StartCoroutine(LogicCoroutine());
            EnabledBehaviours.Add(this);
            health.OnDeath.AddListener(OnDeath);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            EnabledBehaviours.Remove(this);
        }

        private void OnDeath(IStimDamage deathStim)
        {
            // todo: ragdoll death oomph
            ragdoll.transform.parent = null;
            animator.enabled = false;
            ragdoll.RagdollEnabled = true;
            Destroy(gameObject);
        }

        private IEnumerator LogicCoroutine()
        {
            while (true)
            {
                Debug.Log("Ok we made it guys");
                // if we're back here, we just switched to this state
                switch (state)
                {
                    case EnemyState.Idle:
                        animator.Play("Idle");
                        movement.turnTowardsMoveDirection = true;
                        while (true)
                        {
                            movement.Target = _idlePosition + Quaternion.Euler(0, Random.value * 360, 0) * Vector3.forward * idleRadius;
                            yield return new WaitForSeconds(Random.value * (idleDelayMax - idleDelayMin) + idleDelayMin);
                        }
                        break;
                    case EnemyState.Suspicious:
                        break;
                    case EnemyState.Chasing:
                        Debug.Log("Chasing");
                        break;
                    case EnemyState.Attacking:
                        break;
                    case EnemyState.Searching:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}