using UnityEngine;

[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(Rigidbody))]
public class ChargingEnemy : MonoBehaviour
{

    public Animator animator;
    public float maxChargeDistance = 10f;
    public float chargeAccel = 1f;

    // auto-assigned
    private EnemyVision _vision;
    private Rigidbody _rb;
    private PlayerController _player;

    // math stuff
    private bool _charging;
    private bool _closeEnoughToCharge;
    private bool _seesPlayer;

    void Start()
    {
        _vision = GetComponent<EnemyVision>();
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void StartCharging() {
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
    }

    void FixedUpdate()
    {
        if (_charging) {
            _rb.drag = .5f;
            _rb.AddForce(transform.forward * chargeAccel, ForceMode.Force);
        } else {
            _rb.drag = 5;
        }

        Vector3 localVel = transform.InverseTransformVector(_rb.velocity);
        animator.SetFloat("xVel", localVel.x);
        animator.SetFloat("zVel", localVel.z);
    }
}
