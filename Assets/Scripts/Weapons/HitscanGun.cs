using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Weapon))]
public class HitscanGun : MonoBehaviour
{

    public Animator animator;
    public GameObject defaultHitPrefab;
    
    [Header("Gun Mechanics")]
    public float fireDelay = .4f;
    public bool automatic = true;
    public float damage = 5f;
    public float kineticPower = 1000f;
    public int clipSize = 7;
    public float reloadTime = .8f;
    public bool silenced;

    [Header("Feedback")]
    public float kickBack = 3;
    public float kickRotation = 2;

    // auto-assigned
    private Weapon _weapon;
    private CameraKickController _kickController;
    private ChadistAI _chadistAI;
    private PlayerInput _input;
    private InputAction m_Fire;

    // math
    private int _bullets;
    private bool _firing;
    private float _lastShot;
    private bool _reloading;

    private const int BulletsIndex = 0;

    void Start()
    {
        _weapon = GetComponent<Weapon>();
        _kickController = _weapon.playerController.kickController;
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
        // setup callbacks
        _input = _weapon.playerController.input;
        m_Fire = _input.actions["Fire1"];
        _input.actions["Reload"].performed += ReloadInput;
    }

    void OnDestroy() {
        // remove callbacks
        _input.actions["Reload"].performed -= ReloadInput;
    }

    public void ReloadInput(InputAction.CallbackContext ctx) {
        if (!CanFire()) return;
        if (_bullets < clipSize) {
            Reload();
        }
    }

    void Update()
    {
        _bullets = (int)_weapon.GetFloat(HitscanGun.BulletsIndex);
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
                    if (_firing && !automatic) return;
                    Fire();
                    _firing = true;
                    return;
                }
                _firing = false;
            }
        }
    }

    private bool CanFire() => _weapon.CanFire() && Time.time - _lastShot > fireDelay && !_reloading;

    public void SetFireRate(float fireRate)
    {
        fireDelay = 60f / fireRate;
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
        yield return new WaitForSeconds(reloadTime);
        _reloading = false;
        _weapon.SetFloat(HitscanGun.BulletsIndex, clipSize);
    }

    void Fire()
    {
        if (_bullets == 0)
        {
            // play clicking noise?
            return;
        }
        _weapon.SetFloat(HitscanGun.BulletsIndex, _bullets - 1);

        if (!silenced) {
            _chadistAI.SpotPlayer(transform.position);
        }
        
        Ray shotRay = new Ray(_weapon.cam.transform.position, _weapon.cam.transform.rotation * Vector3.forward);
        if (Physics.Raycast(shotRay, out RaycastHit hit, Mathf.Infinity, ~(1 >> 8)))
        {
            // we hit something???
            Transform hitObject = hit.transform;
            Rigidbody hitRB = hitObject.GetComponent<Rigidbody>();
            if (hitRB)
            {
                hitRB.AddForceAtPosition(shotRay.direction * kineticPower, hit.point);
            }

            Shootable shootable = hitObject.GetComponent<Shootable>();
            if (shootable)
            {
                shootable.Shoot(_weapon.playerInventory, _weapon, damage, hit);
                if (shootable.fxPrefab) {
                    SpawnHitFX(shootable.fxPrefab, hit);
                } else {
                    SpawnHitFX(defaultHitPrefab, hit);
                }
            } else {
                // spawn fx
                SpawnHitFX(defaultHitPrefab, hit);
            }


        }
        
        _kickController.AddVel(_weapon.cam.transform.TransformVector(Vector3.forward * (-kickBack)));
        _kickController.AddKick(Quaternion.Euler(-kickRotation, 0, 0));
        
        _lastShot = Time.time;
        animator.SetTrigger("fire");
    }

    void SpawnHitFX(GameObject fxObject, RaycastHit hit) {
        Transform fxTransform = Instantiate(fxObject).transform;
        fxTransform.transform.position = hit.point;
        fxTransform.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
    }

}
