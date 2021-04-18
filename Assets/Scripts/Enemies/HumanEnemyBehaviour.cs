using System;
using System.Collections;
using Player.Controlling;
using ScriptableObjects.Audio;
using ScriptableObjects.Enemies;
using ScriptableObjects.Weapons;
using Stims;
using UnityEngine;
using Weapons;
using Weapons.Enemy;
using World;
using World.StimListeners;
using Random = UnityEngine.Random;

namespace Enemies
{

    public enum EnemyState
    {
        Idle,
        Suspicious,
        Fighting,
        Searching,
    }

    public class HumanEnemyBehaviour : MonoBehaviour
    {

        #region InspectorVariables
        
        [Header("References")]
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private Health health;
        [SerializeField] private EnemyVision vision;
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyWeaponManager weaponManager;
        [SerializeField] private RagdollController ragdoll;
        [SerializeField] private AudioChannel audioChannel;
        
        [Header("Enemy Config")]
        [SerializeField] private HumanEnemyBehaviourConfig config;
        [SerializeField] private WeaponData currentWeapon;
        [SerializeField] private bool startFighting;
        
        #endregion
        
        #region ExposedFields

        public EnemyMovement Movement => movement;
        public EnemyVision Vision => vision;
        public Animator Animator => animator;
        public Vector3 LastKnownPosition => _lastKnownPosition;

        #endregion

        #region StateVariables

        private WeaponState _currentWeaponState;
        private EnemyState _state = EnemyState.Idle;
        private Vector3 _idlePosition;
        private float _suspicion = 1;
        private Vector3 _suspicionPosition;
        private Vector3 _lastKnownPosition;
        private bool _hasWeapon = false;
        private bool _weaponDrawn = false;
        
        #endregion

        #region Initialization

        private void OnEnable()
        {
            // set idle position if we're walking
            if (startFighting)
            {
                _state = EnemyState.Fighting;
            }
            else
            {
                _idlePosition = movement.FeetPos;
            }

            // set up our current weapon prefab if 
            if (currentWeapon != null)
            {
                _hasWeapon = true;
                weaponManager.SetWeaponPrefab(currentWeapon.EnemyBackPrefab, currentWeapon, null);
                Debug.Log("Set up the weapon");
            }
            
            health.OnDeath.AddListener(OnDeath);
            audioChannel.playAudio += OnHearSound;
            
            StartCoroutine(LogicCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            health.OnDeath.RemoveListener(OnDeath);
            audioChannel.playAudio -= OnHearSound;
        }
        
        private void OnDeath(IStimDamage deathStim)
        {
            // todo: drop weapon
            weaponManager.ClearWeaponPrefab();

            if (_hasWeapon) DeathDropWeapon(deathStim);

            // todo: ragdoll death oomph
            ragdoll.RagdollEnabled = true;
            animator.enabled = false;
            ragdoll.transform.parent = null;
            Destroy(gameObject);
        }

        private void DeathDropWeapon(IStimDamage deathStim)
        {
            Vector3 weaponDropForce;
            if (deathStim is PunchDamageStim punchStim)
            {
                weaponDropForce = punchStim.Force();
            }
            else
            {
                weaponDropForce = transform.forward * 20f;
            }

            float weaponGrip = .2f;
            Instantiate(currentWeapon.DropPrefab, transform.position + transform.forward * .6f, transform.rotation)
                .GetComponentInChildren<Rigidbody>().AddForce(weaponDropForce * (1 - weaponGrip), ForceMode.Impulse);
        }

        #endregion

        #region AILogic

        private IEnumerator LogicCoroutine()
        {
            // if we're back here, we just switched to this state
            switch (_state)
            {
                // we don't know the player is around or anything.
                case EnemyState.Idle:

                    PutAwayWeaponIfDrawn();
                    
                    Debug.Log("Starting idle logic");
                    // on initially switching to this state
                    animator.Play("Idle");
                    movement.turnTowardsMoveDirection = true;
                    float switchTime = Time.time;

                    bool canSwitch = false;
                    // "update" loop - break if state switched by sound receiver/other coroutines
                    while (_state == EnemyState.Idle)
                    {
                        
                        if (Time.time >= switchTime)
                        {
                            // loop to move around
                            movement.Target = _idlePosition + Quaternion.Euler(0, Random.value * 360, 0) *
                                Vector3.forward * config.IdleRadius;
                            switchTime = Random.value * (config.IdleDelayMax - config.IdleDelayMin) +
                                               config.IdleDelayMin + Time.time;
                        }
                        
                        // crank up suspicion if we can see player
                        if (vision.CanSeePlayer())
                        {
                            _suspicion += Time.deltaTime * config.SusGainSpeed;
                        }
                        else
                        {
                            // otherwise bring it back to 1
                            _suspicion = Mathf.MoveTowards(_suspicion, 1,
                                Time.deltaTime * config.SusDispelSpeed);
                        }
                        
                        // if we spotted player switch to fighting
                        if (_suspicion > 2)
                        {
                            _state = EnemyState.Fighting;
                        }

                        yield return new WaitForEndOfFrame();
                    }
                    break;
                // we know something is up.. look for suspicious/alarming sounds
                case EnemyState.Suspicious:
                    
                    DrawWeaponIfNotDrawn();
                    
                    Debug.Log("Starting suspicious logic");
                    while (_state == EnemyState.Suspicious)
                    {

                        bool canSeePlayer = vision.CanSeePlayer();
                        
                        // if we can see player, add to suspicion
                        if (canSeePlayer)
                        {
                            _suspicion += config.SusGainSpeed * Time.deltaTime /
                                         Vector3.Magnitude(PlayerController.Main.Orientation.transform.position -
                                                           vision.transform.position);
                        }
                        
                        if (vision.CanSee(_suspicionPosition, .2f))
                        {
                            // we can't see player but we can see pos
                            if (!canSeePlayer)
                            {
                                _suspicion -= config.SusDispelSpeed * Time.deltaTime;
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
                                _suspicion -= .5f * Time.deltaTime;
                            }
                        }

                        if (_suspicion <= 0)
                        {
                            Debug.Log("Switching to idle");
                            _state = EnemyState.Idle;
                        }
                        else if (_suspicion >= 2)
                        {
                            Debug.Log("Switching to fighting");
                            // we saw the player: pull out weapon and set state
                            _state = EnemyState.Fighting;
                        }
                        
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                // we're in combat with the player.
                case EnemyState.Fighting:

                    DrawWeaponIfNotDrawn();
                    
                    Debug.Log("Starting fighting logic");
                    while (_state == EnemyState.Fighting)
                    {
                        
                        GrabWeaponIfNearby();
                        TrackPlayer();

                        if (weaponManager.WeaponComponent)
                        {
                            weaponManager.WeaponComponent.attackMode = _weaponDrawn;
                        }
                        else
                        {
                            Debug.Log("no weapon manager assigned, just going towards player");
                            movement.locked = false;
                            movement.Target = _lastKnownPosition;
                        }

                        yield return new WaitForEndOfFrame();
                    }
                    break;
                // we've lost contact with the player.
                case EnemyState.Searching:
                    // grab a weapon if there's one nearby, look around the map for the player, if we see them switch to attacking
                    // todo: search for player, if we don't find them in time switch to idle
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // if we just got to the end.. just restart the Logic
            StartCoroutine(LogicCoroutine());
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
                    if (_state < EnemyState.Suspicious) _state = EnemyState.Suspicious;

                    // if we have a weapon and it's not drawn, take it out
                    DrawWeaponIfNotDrawn();

                    break;
                case SoundType.Alarming:
                    if (_state < EnemyState.Suspicious)
                    {
                        _state = EnemyState.Suspicious;
                        Debug.Log("Switching to suspicious");
                    }
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
                    if (_state < EnemyState.Suspicious) _state = EnemyState.Suspicious;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
            }
        }

        #endregion

        #region AICoroutines

        private IEnumerator PickUpWeaponCoroutine(Vector3 position)
        {
            movement.Target = position;
            yield return new WaitUntil(() => movement.AtTarget);
            animator.Play("PickupWeapon");
            StartCoroutine(LogicCoroutine());
        }
        
        private bool _drawingWeapon;
        private IEnumerator TakeOutWeaponCoroutine()
        {
            Debug.Log("Taking out weapon");
            _drawingWeapon = true;
            _puttingAwayWeapon = false;
            animator.Play("TakeOutWeapon");
            
            yield return new WaitForSeconds(.5f);
            weaponManager.SetWeaponPrefab(currentWeapon.EnemyPrefab, currentWeapon, _currentWeaponState);
            
            yield return new WaitForSeconds(.3f);
            _weaponDrawn = true;
            _drawingWeapon = false;
        }
        
        private bool _puttingAwayWeapon;
        private IEnumerator PutAwayWeaponCoroutine()
        {
            Debug.Log("Putting Away Weapon");
            _puttingAwayWeapon = true;
            _drawingWeapon = false;
            animator.Play("PutAwayWeapon");
            
            yield return new WaitForSeconds(.5f);
            weaponManager.SetWeaponPrefab(currentWeapon.EnemyBackPrefab, currentWeapon, _currentWeaponState);
            
            yield return new WaitForSeconds(.3f);
            _weaponDrawn = false;
            _puttingAwayWeapon = false;
        }
        
        private IEnumerator DropWeaponCoroutine()
        {
            movement.locked = true;
            animator.Play("DropWeapon");
            _hasWeapon = false;
            _weaponDrawn = false;
            
            yield return new WaitForSeconds(1.5f);
            
            movement.locked = false;
            StartCoroutine(LogicCoroutine());
        }

        #endregion
        
        #region AIFuncs
        
        private void TrackPlayer()
        {
            if (vision.CanSeePlayer())
            {
                _lastKnownPosition = PlayerController.Main.transform.position;
            }
        }
        
        // todo: this
        private void GrabWeaponIfNearby()
        {
            if (_hasWeapon) return;
            // loop through our dudes
            if (false)
            {
                StopAllCoroutines();
                StartCoroutine(PickUpWeaponCoroutine(new Vector3()));
            }
        }
        
        // these guys return true if we are pulling shit out or whatever
        private void DrawWeaponIfNotDrawn()
        {
            StopCoroutine(PutAwayWeaponCoroutine());
            if (_drawingWeapon) return;
            if (!_hasWeapon || _weaponDrawn) return;
            StartCoroutine(TakeOutWeaponCoroutine());
        }
        
        private void PutAwayWeaponIfDrawn()
        {
            StopCoroutine(TakeOutWeaponCoroutine());
            if (_puttingAwayWeapon) return;
            if (!_hasWeapon || !_weaponDrawn) return;
            StartCoroutine(PutAwayWeaponCoroutine());

        }
        
        #endregion

    }
}