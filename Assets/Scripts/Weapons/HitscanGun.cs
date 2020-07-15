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
    public float kineticPower = 1000f;
    public int clipSize = 7;
    public float reloadTime = .8f;

    [Header("Feedback")]
    public float kickBack = 3;
    public float kickRotation = 2;

    // auto-assigned
    private Weapon _weapon;
    private Camera _camera;
    private CameraKickController _kickController;

    // math
    private float _lastShot;
    private bool _reloading;
    public int bullets;

    void Start()
    {
        _weapon = GetComponent<Weapon>();
        _camera = Camera.main;
        _kickController = _weapon.playerController.kickController;
        bullets = _weapon.floats.ContainsKey("bullets") ? Mathf.FloorToInt(_weapon.floats["bullets"]) : clipSize;
    }

    void Update()
    {
        if (_weapon.CanFire())
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
            if (Time.time - _lastShot > fireDelay && firing)
            {
                Fire();
            }
        }
    }

    public void SetFireRate(float fireRate)
    {
        fireDelay = 60f / fireRate;
    }

    void Reload()
    {
        StartCoroutine(ReloadCoroutine());
        animator.SetTrigger("reload");
    }

    IEnumerator ReloadCoroutine()
    {
        _reloading = true;
        yield return new WaitForSeconds(reloadTime);
        _reloading = false;
        bullets = clipSize;
        _weapon.SetFloat("bullets", bullets);
    }

    void Fire()
    {
        if (bullets == 0)
        {
            // play clicking noise?
            return;
        }
        bullets--;
        _weapon.SetFloat("bullets", bullets);
        
        Ray shotRay = new Ray(_camera.transform.position, _camera.transform.rotation * Vector3.forward);
        if (Physics.Raycast(shotRay, out RaycastHit hit, Mathf.Infinity))
        {
            // we hit something???
            Transform hitObject = hit.transform;
            Rigidbody hitRB = hitObject.GetComponent<Rigidbody>();
            if (hitRB)
            {
                hitRB.AddForceAtPosition(shotRay.direction * kineticPower, hit.point);
            }

            Transform fxObject = Instantiate(hitPrefab).transform;
            fxObject.transform.position = hit.point;
            fxObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            // todo: destroy object after a bit

        }
        
        _kickController.AddVel(new Vector3(0, 0, -kickBack));
        _kickController.AddKick(Quaternion.Euler(-kickRotation, 0, 0));
        
        _lastShot = Time.time;
        animator.SetTrigger("fire");
    }

}
