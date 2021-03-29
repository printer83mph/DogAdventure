using System.Collections;
using ScriptableObjects;
using ScriptableObjects.Audio;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using Weapons;

public class HitscanGun : MonoBehaviour
{

    [SerializeField] private Animator animator = null;
    [SerializeField] private GameObject defaultHitPrefab = null;
    [SerializeField] private LayerMask layerMask = (1 << 9) | (1 << 0);
    [SerializeField] private AudioSource audioSource = null;
    
    [Header("Gun Mechanics")] [SerializeField]
    private HitscanGunData gunData = null;

    [Header("Feedback")]
    [SerializeField] private float kickBack = 3;
    [SerializeField] private float kickRotation = 2;

    // auto-assigned
    private Weapon _weapon;
    // private CameraKickController _kickController;
    private PlayerInput _input;
    private InputAction m_Fire;

    // math
    private int _bullets;
    private bool _firing;
    private float _lastShot;
    private bool _reloading;

    private const int BulletsIndex = 0;

    private void Awake() {
        _weapon = GetComponent<Weapon>();
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

    private float GetDamage(float distance)
    {
        Debug.Log(gunData.BaseDamage * (1 - Mathf.Pow(distance / gunData.MaxRange, gunData.FalloffExponent)));
        return gunData.BaseDamage * (1 - Mathf.Pow(distance / gunData.MaxRange, gunData.FalloffExponent));
    }

    void Update()
    {
        _bullets = _weapon.InventoryState.GetInt(HitscanGun.BulletsIndex);
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
        _weapon.InventoryState.SetInt(BulletsIndex, gunData.ClipSize);
    }

    void Fire()
    {
        if (_bullets == 0)
        {
            // play clicking noise?
            return;
        }
        _weapon.InventoryState.SetInt(BulletsIndex, _bullets - 1);

        // audio event
        gunData.AudioChannel.PlayEvent(gunData.AudioEvent);

        Transform shotTransform = _weapon.Controller.Orientation;
        
        Ray shotRay = new Ray(shotTransform.position, shotTransform.rotation * Vector3.forward);
        if (Physics.Raycast(shotRay, out RaycastHit hit, gunData.MaxRange, layerMask))
        {
            // we hit something???
            Transform hitObject = hit.transform;
            Rigidbody hitRB = hitObject.GetComponent<Rigidbody>();
            if (hitRB)
            {
                hitRB.AddForceAtPosition(shotRay.direction * gunData.KineticPower, hit.point);
            }

            Damageable damageable = hitObject.GetComponent<Damageable>();
            if (damageable)
            {
                damageable.Damage(new Damage.PlayerBulletDamage(GetDamage(hit.distance), shotRay.direction, hit));
                if (damageable.fxPrefab) {
                    SpawnHitFX(damageable.fxPrefab, hit);
                } else {
                    SpawnHitFX(defaultHitPrefab, hit);
                }
            } else {
                // spawn fx
                SpawnHitFX(defaultHitPrefab, hit);
            }


        }
        
        // _kickController.AddVel(_weapon.cam.transform.TransformVector(Vector3.forward * (-kickBack)));
        // _kickController.AddKick(Quaternion.Euler(-kickRotation, 0, 0));
        
        _lastShot = Time.time;
        animator.SetTrigger("fire");
    }

    void SpawnHitFX(GameObject fxObject, RaycastHit hit) {
        Transform fxTransform = Instantiate(fxObject).transform;
        fxTransform.transform.position = hit.point;
        fxTransform.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
    }

}
