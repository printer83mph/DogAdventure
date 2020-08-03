using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class ChargingEnemy : MonoBehaviour
{

    public Animator animator;
    public float maxChargeDistance = 10f;
    public float chargeDistanceBuffer = 1f;
    public float chargeAccel = 1f;
    public float turnSpeed = 2f;

    // auto-assigned
    private EnemyVision _vision;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private PlayerController _player;
    private ChadistAI _chadistAI;

    // math stuff
    private bool _charging;
    private bool _closeEnoughToCharge;
    private bool _seesPlayer;

    void Start()
    {
        _vision = GetComponent<EnemyVision>();
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void StartCharge() {
        _charging = true;
        _rb.isKinematic = false;
        _agent.enabled = false;
    }

    void Update() {
        bool seesPlayer = _vision.CanSeePlayer;
        // give us some buffer room
        float calcMaxChargeDistance = maxChargeDistance + (_closeEnoughToCharge ? chargeDistanceBuffer : 0);
        bool closeEnoughToCharge = seesPlayer && (_vision.PlayerDistance < calcMaxChargeDistance);
        if (seesPlayer != _seesPlayer) {
            animator.SetBool("seesPlayer", seesPlayer);
            _seesPlayer = seesPlayer;
        }
        if (closeEnoughToCharge != _closeEnoughToCharge) {
            animator.SetBool("closeEnoughToCharge", closeEnoughToCharge);
            _closeEnoughToCharge = closeEnoughToCharge;
        }

        if (_seesPlayer) {
            _chadistAI.SpotPlayer(_player.transform.position);
        }

        if (!_charging && _chadistAI.alertStatus != 0) {
            MoveTowardsPlayer();
            if (_closeEnoughToCharge) RotateTowardsPlayer();
        }

        Vector3 localVel = transform.InverseTransformVector(_agent.velocity);
        animator.SetFloat("xVel", localVel.x);
        animator.SetFloat("zVel", localVel.z);
    }

    private void MoveTowardsPlayer() {
        if (_closeEnoughToCharge) {
            _agent.isStopped = true;
        } else {
            _agent.isStopped = false;
            _agent.destination = _chadistAI.lastKnownPos;
        }
    }

    private void RotateTowardsPlayer() {
        Vector3 vecToPlayer = _player.transform.position - transform.position;
        vecToPlayer.y = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vecToPlayer), turnSpeed * Time.deltaTime);
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
