﻿using System.Collections;
using ScriptableObjects;
using ScriptableObjects.Audio;
using ScriptableObjects.Audio.Events;
using Stims;
using Stims.Effectors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons.Guns
{

    public class HitscanGun : MonoBehaviour
    {
        private enum StateInts
        {
            Bullets
        }
        
        [SerializeField] private Animator animator = null;
    
        [Header("Gun Mechanics")] [SerializeField]
        private HitscanGunData gunData = null;

        // auto-assigned
        private Weapon _weapon;
        private HitscanEffector _hitscanEffector;
        // private CameraKickController _kickController;
        private PlayerInput _input;
        private InputAction m_Fire;

        // math
        private int _bullets;
        private bool _firing;
        private float _lastShot;
        private bool _reloading;
        

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
            _hitscanEffector = GetComponent<HitscanEffector>();
        }

        private void Start()
        {
            // _kickController = _weapon.Controller.kickController;
            // setup callbacks
            _input = _weapon.Input;
            m_Fire = _input.actions["Fire1"];
            _input.actions["Reload"].performed += ReloadInput;
        }

        private void OnDestroy() {
            // remove callbacks
            if (_input) _input.actions["Reload"].performed -= ReloadInput;
            StopAllCoroutines();
        }

        public void ReloadInput(InputAction.CallbackContext ctx) {
            if (!CanFire()) return;
            if (_bullets < gunData.ClipSize) {
                Reload();
            }
        }

        void Update()
        {
            _bullets = _weapon.State.GetInt((int)StateInts.Bullets);
            if (Time.time - _lastShot > fireDelay + .03f)
            {
                if (gunData.AudioEvent is ContinuousAudioEvent)
                {
                    gunData.AudioChannel.Stop();
                }
            }
            if (CanFire())
            {
                if (_bullets == 0) {
                    Reload();
                } else {
                    float pressure = m_Fire.ReadValue<float>();
                    bool held = pressure > .5f;
                    animator.SetBool("trigger", held);
                    if (held) {
                        // return if we're holding fire on semi
                        if (_firing && !gunData.Automatic) return;
                        Fire();
                        _firing = true;
                        return;
                    }
                    _firing = false;
                }
            }
        }

        private bool CanFire() => Time.time - _lastShot > fireDelay && !_reloading && !_weapon.Equipping;

        private float fireDelay => 1.0f / gunData.FireRate;

        void Fire()
        {
            if (_bullets == 0) return;
            // audio event
            gunData.AudioChannel.PlayEvent(gunData.AudioEvent);
            // run hitscan shooter guy here
            _hitscanEffector.Shoot(gunData.BaseDamage, gunData.BaseForce,
                new FalloffEffect.LimitedExponential(gunData.FalloffExponent, gunData.MaxRange),
                StimSource.Generic.Player,
                // raycast data
                maxRange: gunData.MaxRange, shotTransform: _weapon.Controller.Orientation,
                // overrides
                overrideHitPrefab: gunData.HitPrefab, overrideHitAudioEvent: gunData.HitAudioEvent);
            _lastShot = Time.time;
            animator.SetTrigger("fire");
            _weapon.State.SetInt((int)StateInts.Bullets, _bullets - 1);
        }
        
        void Reload()
        {
            _firing = false;
            animator.SetBool("trigger", false);
            StartCoroutine(ReloadCoroutine());
            animator.SetTrigger("reload");
        }

        IEnumerator ReloadCoroutine()
        {
            _reloading = true;
            yield return new WaitForSeconds(gunData.ReloadTime);
            _reloading = false;
            _weapon.State.SetInt((int)StateInts.Bullets, gunData.ClipSize);
        }

    }
}
