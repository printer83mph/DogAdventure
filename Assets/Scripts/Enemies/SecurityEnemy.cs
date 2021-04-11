using ScriptableObjects;
using Stims;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StimReceiver))]
[RequireComponent(typeof(EnemyBehaviour))]
public class SecurityEnemy : MonoBehaviour
{

    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform aimBone = null;
    [SerializeField] private Transform eyeTransform = null;
    [SerializeField] private AudioEvent audioEvent = null;
    [SerializeField] private AudioSource audioSource = null;

    public float gunDamage = .4f;
    
    // auto-assigned
    private StimReceiver _stimReceiver = null;
    private EnemyBehaviour _behaviour = null;
    private EnemyHealth _health = null;
    private PlayerController _player = null;
    private PlayerHealth _playerHealth = null;
    private float _nextLook;

    void Awake()
    {
        _stimReceiver = GetComponent<StimReceiver>();
        _behaviour = GetComponent<EnemyBehaviour>();
        _health = GetComponent<EnemyHealth>();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
    }

    void OnEnable() {
        _stimReceiver.AddStimListener(OnStim);
        _health.onDeath += OnDeath;
        _behaviour.onAttackUpdate += OnAttackUpdate;
    }

    void OnDisable() {
        _stimReceiver.RemoveStimListener(OnStim);
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
        // _playerHealth.Damage(new Stim.(gunDamage, new EnemyDamageSource(_behaviour, "a Chadist Goon"),
        //     _player.transform.position - eyeTransform.position));
        // _player.GetComponent<CameraKickController>().AddKick(Quaternion.Euler(-5,0,3));
        audioEvent.Play(audioSource);
    }
    
    private void OnAttackUpdate(bool canAttack)
    {
        animator.SetBool("canShoot", canAttack);
    }

    private void OnStim(Stim stim)
    {
        if (!(stim is IStimDamage)) return;
        animator.SetTrigger("flinch");
    }

    private void OnDeath(Stim stim) {
        animator.SetTrigger("death");
        this.enabled = false;
    }
}
