using System.Collections;
using ScriptableObjects;
using ScriptableObjects.Audio;
using Stims;
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
        private HitscanShooter _hitscanShooter;
        // private CameraKickController _kickController;
        private PlayerInput _input;
        private InputAction m_Fire;

        // math
        private int _bullets;
        private bool _firing;
        private float _lastShot;
        private bool _reloading;
        

        private void Awake() {
            _weapon = GetComponent<Weapon>();
            _hitscanShooter = GetComponent<HitscanShooter>();
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
        }

        public void ReloadInput(InputAction.CallbackContext ctx) {
            if (!CanFire()) return;
            if (_bullets < gunData.ClipSize) {
                Reload();
            }
        }

        void Update()
        {
            _bullets = _weapon.InventoryState.GetInt((int)StateInts.Bullets);
            // todo: convert to coroutines so audio doesnt get fucked
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

        // private bool CanFire() => _weapon.CanFire() && Time.time - _lastShot > fireDelay && !_reloading;
        // todo: make equiphandler component

        private bool CanFire() => Time.time - _lastShot > fireDelay && !_reloading;

        private float fireDelay => 1.0f / gunData.FireRate;

        void Fire()
        {
            if (_bullets == 0) return;
            // audio event
            gunData.AudioChannel.PlayEvent(gunData.AudioEvent);
            // run hitscan shooter guy here
            _hitscanShooter.Shoot(gunData.BaseDamage, gunData.BaseForce,
                new FalloffEffect.LimitedExponential(gunData.FalloffExponent, gunData.MaxRange),
                StimSource.Generic.Player,
                // raycast data
                maxRange: gunData.MaxRange, shotTransform: _weapon.Controller.Orientation,
                // overrides
                overrideHitPrefab: gunData.HitPrefab, overrideHitAudioEvent: gunData.HitAudioEvent);
            _lastShot = Time.time;
            animator.SetTrigger("fire");
            _weapon.InventoryState.SetInt((int)StateInts.Bullets, _bullets - 1);
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
            _weapon.InventoryState.SetInt((int)StateInts.Bullets, gunData.ClipSize);
        }

    }
}
