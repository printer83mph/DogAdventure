using System;
using System.Collections;
using System.Collections.Generic;
using Player.Controlling;
using ScriptableObjects.Audio;
using ScriptableObjects.Enemies;
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
        [SerializeField] private AudioChannel audioChannel;

        [SerializeField] private HumanEnemyBehaviourConfig config;
        [SerializeField] private EnemyState state = EnemyState.Idle;

        private Vector3 _idlePosition;
        private Vector3 _suspicionPosition;
        private Vector3 _lastKnownPosition;
        private bool _hasWeapon;
        private bool _weaponDrawn;

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
            audioChannel.playAudio += OnHearSound;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            EnabledBehaviours.Remove(this);
            health.OnDeath.RemoveListener(OnDeath);
            audioChannel.playAudio -= OnHearSound;
        }

        private void OnDeath(IStimDamage deathStim)
        {
            // todo: ragdoll death oomph
            ragdoll.RagdollEnabled = true;
            animator.enabled = false;
            ragdoll.transform.parent = null;
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
                        while (state == EnemyState.Idle)
                        {
                            movement.Target = _idlePosition + Quaternion.Euler(0, Random.value * 360, 0) *
                                Vector3.forward * config.IdleRadius;
                            
                            float switchTime = Random.value * (config.IdleDelayMax - config.IdleDelayMin) +
                                            config.IdleDelayMin + Time.time;
                            yield return new WaitUntil(() => Time.time > switchTime || state != EnemyState.Idle);
                        }
                        // todo: let enemy see player here lol
                        break;
                    case EnemyState.Suspicious:
                        float suspicion = 1;
                        while (state == EnemyState.Suspicious)
                        {
                            bool canSeePlayer = vision.CanSee(PlayerController.Main.transform.position);
                            
                            // if we can see player, add to suspicion
                            if (canSeePlayer)
                            {
                                suspicion += config.SusGainSpeed * Time.deltaTime /
                                             Vector3.Magnitude(PlayerController.Main.transform.position -
                                                               vision.transform.position);
                            }
                            
                            if (vision.CanSee(_suspicionPosition, .2f))
                            {
                                // we can't see player but we can see pos
                                if (!canSeePlayer)
                                {
                                    suspicion -= config.SusDispelSpeed * Time.deltaTime;
                                }
                                
                                // stay still
                                movement.Target = movement.FeetPos;
                            }
                            else
                            {
                                // we can't see the suspicious target, so walk until we can
                                movement.Target = _suspicionPosition;
                                if (Vector3.SqrMagnitude(transform.position - _suspicionPosition) < 1)
                                {
                                    suspicion -= .5f * Time.deltaTime;
                                }
                            }

                            Debug.Log("Suspicion: " + suspicion);
                            if (suspicion <= 0)
                            {
                                Debug.Log("Switching to idle");
                                state = EnemyState.Idle;
                                break;
                            }
                            if (suspicion >= 2)
                            {
                                // we saw the player: pull out weapon and set state
                                
                            }
                            yield return new WaitForEndOfFrame();
                        }
                        break;
                    case EnemyState.Chasing:
                        // grab a weapon if there's one nearby, chase the player until we can attack them
                        while (true)
                        {
                            GrabWeaponIfNearby();
                            TrackPlayer();
                            movement.Target = _lastKnownPosition;
                            yield return new WaitForEndOfFrame();
                        }
                        break;
                    case EnemyState.Attacking:
                        // grab a weapon if there's one nearby, and attack the player.
                        GrabWeaponIfNearby();
                        TrackPlayer();
                        // todo: attack player, if we can't see player for a bit we start searching, if we're too far away switch to chasing
                        break;
                    case EnemyState.Searching:
                        // grab a weapon if there's one nearby, look around the map for the player, if we see them switch to attacking
                        // todo: search for player, if we don't find them in time switch to idle
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void TrackPlayer()
        {
            if (vision.CanSee(PlayerController.Main.transform.position))
            {
                _lastKnownPosition = PlayerController.Main.transform.position;
            }
        }
        
        // todo: this
        private void GrabWeaponIfNearby()
        {
            // loop through our dudes
            if (false)
            {
                StartCoroutine(PickUpWeaponCoroutine(new Vector3()));
            }
        }

        private void TryTakeOutWeapon()
        {
            if (_hasWeapon && _weaponDrawn)
            {
                StartCoroutine(TakeOutWeaponCoroutine());
            }
        }

        private void OnHearSound(Vector3 position, SoundType soundType, float radius)
        {
            // dont give a shit if unimportant
            if (soundType == SoundType.Unimportant) return;
            
            // dont give a shit if outside radius
            if (Vector3.SqrMagnitude(transform.position - position) > Mathf.Pow(radius, 2)) return;
            
            Debug.Log("So we heard the sound..");
            
            // set our suspicion position
            _suspicionPosition = position;
            
            switch (soundType)
            {
                case SoundType.Suspicious:
                    // if we're idle, switch to suspicious
                    if (state < EnemyState.Suspicious) state = EnemyState.Suspicious;

                    // if we have a weapon and it's not drawn, take it out
                    TryTakeOutWeapon();

                    break;
                case SoundType.Alarming:
                    if (state < EnemyState.Suspicious)
                    {
                        state = EnemyState.Suspicious;
                        Debug.Log("Switching to suspicious");
                    }

                    // alarming sounds make us draw weapon
                    TryTakeOutWeapon();
                    break;
                case SoundType.Stunning:
                    // stunning sounds make us drop weapon
                    if (_hasWeapon)
                    {
                        StartCoroutine(DropWeaponCoroutine());
                    }
                    else
                    {
                        // todo: falter animation
                    }

                    // if we're not already chasing, chase
                    if (state < EnemyState.Suspicious) state = EnemyState.Suspicious;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
            }
        }

        private IEnumerator PickUpWeaponCoroutine(Vector3 position)
        {
            StopAllCoroutines();
            movement.Target = position;
            yield return new WaitUntil(() => movement.AtTarget);
            animator.Play("PickupWeapon");
            StartCoroutine(LogicCoroutine());
        }
        
        // animator coroutines
        private IEnumerator TakeOutWeaponCoroutine()
        {
            StopAllCoroutines();
            _hasWeapon = true;
            animator.Play("TakeOutWeapon");
            yield return new WaitForSeconds(.8f);
            _weaponDrawn = true;
            StartCoroutine(LogicCoroutine());
        }
        
        // animator coroutines
        private IEnumerator DropWeaponCoroutine()
        {
            StopAllCoroutines();
            movement.locked = true;
            animator.Play("DropWeapon");
            _hasWeapon = false;
            _weaponDrawn = false;
            
            yield return new WaitForSeconds(1.5f);
            
            movement.locked = false;
            StartCoroutine(LogicCoroutine());
        }

    }
}