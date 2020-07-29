using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(Rigidbody))]
public class ChargingEnemy : MonoBehaviour
{

    public Animator animator;
    public float maxChargeDistance = 10f;
    public float chargeAccel = 1f;
    public float turnSpeed = 2f;

    // auto-assigned
    private EnemyVision _vision;
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
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void StartCharge() {
        _charging = true;
    }

    void Update() {
        bool seesPlayer = _vision.CanSeePlayer;
        bool closeEnoughToCharge = _vision.PlayerLOSDistance < maxChargeDistance && seesPlayer;
        if (seesPlayer != _seesPlayer) {
            animator.SetBool("seesPlayer", seesPlayer);
            _seesPlayer = seesPlayer;
        }
        if (closeEnoughToCharge != _closeEnoughToCharge) {
            animator.SetBool("closeEnoughToCharge", closeEnoughToCharge);
            _closeEnoughToCharge = closeEnoughToCharge;
        }

        if (!_charging && _chadistAI.alertStatus != 0) {
            RotateTowardsPlayer();
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
            _rb.drag = 25f;
        }

        Vector3 localVel = transform.InverseTransformVector(_rb.velocity);
        animator.SetFloat("xVel", localVel.x);
        animator.SetFloat("zVel", localVel.z);
    }
}
