using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(EnemyBehaviour))]
public class SecurityEnemy : MonoBehaviour
{

    public Animator animator;
    public Transform aimBone;
    public Transform eyeTransform;

    public float gunDamage = .4f;
    
    // auto-assigned
    private Damageable _damageable;
    private EnemyBehaviour _behaviour;
    private EnemyHealth _health;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    private ChadistAI _chadistAI;
    private float _nextLook;

    void Awake()
    {
        _damageable = GetComponent<Damageable>();
        _behaviour = GetComponent<EnemyBehaviour>();
        _health = GetComponent<EnemyHealth>();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _chadistAI = GameObject.FindWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void OnEnable() {
        _damageable.onDamage += OnDamage;
        _health.onDeath += OnDeath;
        _behaviour.onAttackUpdate += OnAttackUpdate;
    }

    void OnDisable() {
        _damageable.onDamage -= OnDamage;
        _health.onDeath -= OnDeath;
        _behaviour.onAttackUpdate -= OnAttackUpdate;
    }

    void Start() {
        _nextLook = Time.time + Random.Range(2, 5);
    }

    private void Update()
    {
        // flinch layer info
        bool flinching = animator.GetCurrentAnimatorStateInfo(1).IsName("Flinch");
        
        if (Time.time > _nextLook && !_behaviour.CanSeePlayer) {
            animator.SetTrigger("lookAround");
            _nextLook = Time.time + Random.Range(2, 5); 
        }

        _behaviour.SetLocked(flinching);
        _behaviour.turnTowardsPlayer = !flinching;

        Vector3 vel = transform.InverseTransformVector(_behaviour.AgentVelocity) * (1/_behaviour.Agent.speed);
        animator.SetFloat("xVel", vel.x);
        animator.SetFloat("zVel", vel.z);
    }

    private void LateUpdate() {
        AimSpineBone();
    }

    void AimSpineBone()
    {
        Quaternion requiredRotation = Quaternion.LookRotation(
            transform.InverseTransformDirection(_player.cam.transform.position - eyeTransform.position), Vector3.up);
        aimBone.rotation *= Quaternion.Slerp(Quaternion.identity, requiredRotation, animator.GetFloat("AimAccuracy"));
    }

    private void ShootPlayer()
    {
        _playerHealth.Damage(new Damage.BulletDamage(gunDamage, new EnemyDamageSource(_behaviour, "a Chadist Goon"),
            _player.transform.position - eyeTransform.position));
        _player.GetComponent<CameraKickController>().AddKick(Quaternion.Euler(-5,0,3));
    }
    
    private void OnAttackUpdate(bool canAttack)
    {
        animator.SetBool("canShoot", canAttack);
    }

    private void OnDamage(Damage damage)
    {
        animator.SetTrigger("flinch");
    }

    private void OnDeath(Damage damage) {
        animator.SetTrigger("death");
        this.enabled = false;
    }
}
