using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class HitscanGun : MonoBehaviour
{

    public Animator animator;
    public GameObject hitPrefab;
    
    [Header("Gun Mechanics")]
    public float fireDelay = .4f;
    public bool automatic = true;
    public float damage = 5f;
    public float kineticPower = 1000f;
    public int clipSize = 7;
    public float reloadTime = .8f;

    [Header("Feedback")]
    public float kickBack = 3;
    public float kickRotation = 2;

    // auto-assigned
    private Weapon _weapon;
    private CameraKickController _kickController;

    // math
    private float _lastShot;
    private bool _reloading;

    private const int BulletsIndex = 0;

    void Start()
    {
        _weapon = GetComponent<Weapon>();
        _kickController = _weapon.playerController.kickController;
    }

    void Update()
    {
        int bullets = (int)_weapon.GetFloat(HitscanGun.BulletsIndex);
        if (_weapon.CanFire() && Time.time - _lastShot > fireDelay)
        {
            if (_reloading) return;
            if ((Input.GetAxis("Reload") > 0 && bullets < clipSize) || bullets == 0)
            {
                Reload();
                return;
            }
            
            bool trigger = Input.GetMouseButton(0);
            bool firing = trigger;
            if (!automatic) firing = Input.GetMouseButtonDown(0);
            animator.SetBool("trigger", trigger);
            if (firing)
            {
                Fire(bullets);
            }
        }
    }

    public void SetFireRate(float fireRate)
    {
        fireDelay = 60f / fireRate;
    }

    void Reload()
    {
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

    void Fire(int bullets)
    {
        if (bullets == 0)
        {
            // play clicking noise?
            return;
        }
        _weapon.SetFloat(HitscanGun.BulletsIndex, bullets - 1);
        
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
            }

            // spawn fx
            Transform fxObject = Instantiate(hitPrefab).transform;
            fxObject.transform.position = hit.point;
            fxObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

        }
        
        _kickController.AddVel(_weapon.cam.transform.TransformVector(Vector3.forward * (-kickBack)));
        _kickController.AddKick(Quaternion.Euler(-kickRotation, 0, 0));
        
        _lastShot = Time.time;
        animator.SetTrigger("fire");
    }

}
