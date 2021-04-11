using Stims;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyBehaviour))]
[RequireComponent(typeof(Rigidbody))]
public class ChargingEnemy : MonoBehaviour
{

    public Animator animator;
    public float chargeDistanceBuffer = 1f;
    public float chargeAccel = 1f;

    // auto-assigned
    private EnemyBehaviour _behaviour;
    private EnemyVision _vision;
    private Rigidbody _rb;
    private EnemyHealth _health;

    private PlayerController _player;
    private ChadistAI _chadistAI;

    // math stuff
    private bool _charging;
    private float _maxChargeDistance;

    void Awake()
    {
        _behaviour = GetComponent<EnemyBehaviour>();
        _vision = GetComponent<EnemyVision>();
        _rb = GetComponent<Rigidbody>();
        _health = GetComponent<EnemyHealth>();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void OnEnable() {
        _behaviour.onAttackUpdate += OnVision;
        _health.onDeath += OnDeath;
        _vision.onVisionUpdate += OnAttack;
    }

    void OnDisable() {
        _behaviour.onAttackUpdate -= OnVision;
        _health.onDeath -= OnDeath;
        _vision.onVisionUpdate -= OnAttack;
    }

    void Start() {
        _behaviour.turnTowardsPlayer = true;
        _maxChargeDistance = _behaviour.attackingDistance;
    }

    void StartCharge() {
        _charging = true;
        _behaviour.turnTowardsPlayer = false;
        _rb.isKinematic = false;
        _behaviour.SetLocked(true);
    }

    void StopCharge() {
        _charging = false;
        _behaviour.turnTowardsPlayer = true;
        _rb.isKinematic = true;
        _behaviour.SetLocked(false);
        animator.SetTrigger("hitObstacle");
    }

    void Update() {
        bool seesPlayer = _behaviour.CanSeePlayer;
        // give us some buffer room
        _behaviour.attackingDistance = _maxChargeDistance + (_behaviour.Attacking ? chargeDistanceBuffer : 0);
        bool closeEnoughToCharge = _behaviour.Attacking;

        Vector3 localVel = transform.InverseTransformVector(_behaviour.AgentVelocity);
        animator.SetFloat("xVel", localVel.x);
        animator.SetFloat("zVel", localVel.z);
    }

    void OnVision(bool seesPlayer) {
        animator.SetBool("seesPlayer", seesPlayer);
    }

    void OnAttack(bool canAttack) {
        animator.SetBool("closeEnoughToCharge", canAttack);
    }

    void FixedUpdate()
    {
        if (_charging) {
            _rb.drag = 10f;
            _rb.AddForce(transform.forward * chargeAccel, ForceMode.Force);
        } else {
            
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_charging) return;
        ContactPoint colPoint = other.GetContact(0);
        if (Vector3.Angle(colPoint.normal, transform.forward) > 105) {
            Debug.Log(Vector3.Angle(colPoint.normal, transform.forward));
            StopCharge();
        }
    }

    private void OnDeath(Stim stim)
    {
        this.enabled = false;
        // TODO: PLACEHOLDER
        // Destroy(gameObject);
    }
}
