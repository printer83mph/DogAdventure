using System.Collections;
using Player.Controlling;
using ScriptableObjects.Audio.Events;
using ScriptableObjects.Weapons;
using Stims;
using Stims.Effectors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons.Player
{

    public class HitscanGun : MonoBehaviour
    {
        
        [SerializeField] private Animator animator = null;
    
        [Header("Gun Mechanics")]
        [SerializeField] private PlayerHitscanGunData gunData = null;

        // auto-assigned
        private PlayerInventoryWeapon _weapon;
        private GunWeaponState _gunState;
        private HitscanEffector _hitscanEffector;
        // private CameraKickController _kickController;
        private InputAction m_Fire;

        // math
        private int _bullets;
        private bool _firing;
        private float _lastShot;
        private bool _reloading;
        

        private void Awake()
        {
            _weapon = GetComponent<PlayerInventoryWeapon>();
            _hitscanEffector = GetComponent<HitscanEffector>();
            m_Fire = PlayerController.Input.actions["Fire1"];
        }

        private void OnEnable()
        {
            PlayerController.Input.actions["Reload"].performed += ReloadInput;
        }

        private void OnDisable()
        {
            PlayerController.Input.actions["Reload"].performed -= ReloadInput;
            StopAllCoroutines();
        }

        private void Start()
        {
            _weapon.State ??= new GunWeaponState(gunData.ClipSize);
            _gunState = (GunWeaponState) _weapon.State;
        }

        private void ReloadInput(InputAction.CallbackContext ctx) {
            if (!CanFire()) return;
            if (_bullets < gunData.ClipSize) {
                Reload();
            }
        }

        void Update()
        {
            _bullets = _gunState.bullets;
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
                maxRange: gunData.MaxRange, shotTransform: PlayerController.Main.Camera.transform,
                // overrides
                overrideHitPrefab: gunData.HitPrefab, overrideHitAudioEvent: gunData.HitAudioEvent);
            
            // visuals
            animator.SetTrigger("fire");
            PlayerController.Main.CameraAdjuster.AddKick(gunData.KickData.GetKick(_lastShot));
                
            // update state
            _lastShot = Time.time;
            _gunState.bullets = _bullets - 1;
            
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
            _gunState.bullets = gunData.ClipSize;
        }

    }
}
