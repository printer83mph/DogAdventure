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
    private Rigidbody _rb;
    private PlayerController _player;
    private ChadistAI _chadistAI;

    // math stuff
    private bool _charging;
    private bool _closeEnoughToCharge;
    private bool _seesPlayer;
    private float _maxChargeDistance;

    void Awake()
    {
        _behaviour = GetComponent<EnemyBehaviour>();
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void Start() {
        _maxChargeDistance = _behaviour.attackingDistance;
    }

    void StartCharge() {
        _charging = true;
        _rb.isKinematic = false;
        _behaviour.SetLocked(true);
    }

    void Update() {
        bool seesPlayer = _behaviour.CanSeePlayer;
        // give us some buffer room
        _behaviour.attackingDistance = _maxChargeDistance + (_closeEnoughToCharge ? chargeDistanceBuffer : 0);
        bool closeEnoughToCharge = _behaviour.CanAttack;
        if (seesPlayer != _seesPlayer) {
            animator.SetBool("seesPlayer", seesPlayer);
            _seesPlayer = seesPlayer;
        }
        if (closeEnoughToCharge != _closeEnoughToCharge) {
            animator.SetBool("closeEnoughToCharge", closeEnoughToCharge);
            _closeEnoughToCharge = closeEnoughToCharge;
        }

        Vector3 localVel = transform.InverseTransformVector(_behaviour.AgentVelocity);
        animator.SetFloat("xVel", localVel.x);
        animator.SetFloat("zVel", localVel.z);
    }

    void FixedUpdate()
    {
        if (_charging) {
            _rb.drag = 10f;
            _rb.AddForce(transform.forward * chargeAccel, ForceMode.Force);
        } else {
            
        }
    }
}
