using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class HitscanGun : MonoBehaviour
{

    public Animator animator;
    public float fireDelay = .4f;
    public bool automatic = true;

    // auto-assigned
    private Weapon _weapon;

    // math
    private float _lastShot;

    void Start()
    {
        _weapon = GetComponent<Weapon>();
    }

    void Update()
    {
        if (_weapon.CanFire())
        {
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

    void SetFireRate(float fireRate)
    {
        fireDelay = 60f / fireRate;
    }

    void Fire()
    {
        _lastShot = Time.time;
        animator.SetTrigger("fire");
    }
}
