using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Shootable))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyVision))]
public class SecurityEnemy : MonoBehaviour
{

    public Animator animator;
    public Transform aimBone;
    public Transform eyeTransform;

    public float maxShootDistance = 7f;
    public float walkRunScaler = .2f;

    public float gunDamage = .4f;
    public float health = 10;
    
    // auto-assigned
    private EnemyVision _vision;
    private NavMeshAgent _agent;
    private PlayerController _player;
    private PlayerHealth _health;
    private ChadistAI _chadistAI;

    // math stuff
    private bool _dead;
    private bool _engaging;
    private AnimatorStateInfo _info;

    static int numEngagingPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Shootable>().onShootDelegate += OnShoot;
        _vision = GetComponent<EnemyVision>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _health = _player.GetComponent<PlayerHealth>();
        _chadistAI = GameObject.FindWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    private void TryEngage() {
        if (!_engaging && numEngagingPlayer < _chadistAI.maxSecurityEngaging) {
            _engaging = true;
            numEngagingPlayer ++;
            animator.SetBool("canShoot", true);
            Debug.Log(numEngagingPlayer + " engaging");
        }
    }

    private void TryDisengage() {
        if (_engaging) {
            _engaging = false;
            numEngagingPlayer --;
            animator.SetBool("canShoot", false);
            Debug.Log(numEngagingPlayer + " engaging");
        }
    }

    private void Update()
    {
        if (_dead) return;
        
        // flinch layer info
        _info = animator.GetCurrentAnimatorStateInfo(1);
        
        // update line of sight and spotting data
        bool canSeePlayer = _vision.CanSeePlayer;
        animator.SetBool("searching", !canSeePlayer);
        float playerDistance = _vision.PlayerLOSDistance;

        _agent.SetDestination(_chadistAI.lastKnownPos);

        // block moving if we have max engaging or we're flinching
        bool blockedFromMoving = numEngagingPlayer == _chadistAI.maxSecurityEngaging || _info.IsName("Flinch");

        if (canSeePlayer) {

            // update chadist AI if player is spotted
            _chadistAI.SpotPlayer(_player.transform.position);
            
            if (playerDistance < maxShootDistance) {
                // we are able to shoot the player, but do we?
                TryEngage();
            } else {
                // we can see them but can't shoot - disengage
                TryDisengage();
            }
        } else {

            // cannot see player - disengage
            TryDisengage();

        }

        _agent.isStopped = _engaging || blockedFromMoving;
        
        if (_chadistAI.alertStatus == 0)
        {
            _agent.isStopped = true;
        } else if (!_info.IsName("Flinch") && _agent.isStopped) {
            RotateTowardsPlayer();
        }

        Vector3 vel = transform.InverseTransformVector(_agent.velocity) * (1/_agent.speed);
        animator.SetFloat("xVel", vel.x);
        animator.SetFloat("zVel", vel.z);
    }

    void AimSpineBone() {
        Quaternion requiredRotation = Quaternion.LookRotation(transform.InverseTransformDirection(_player.cam.transform.position - eyeTransform.position), Vector3.up);
        aimBone.rotation *= Quaternion.Slerp(Quaternion.identity, requiredRotation, animator.GetFloat("AimAccuracy"));
    }

    private void LateUpdate() {
        AimSpineBone();
    }

    void RotateTowardsPlayer()
    {
        Vector3 plyPos = _player.transform.position;
        Vector2 xzPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 plyxzPos = new Vector2(plyPos.x, plyPos.z);
        transform.rotation = PrintUtil.Damp(transform.rotation,
            Quaternion.Euler(0,
                Quaternion.LookRotation(plyPos - transform.position, Vector3.up).eulerAngles.y, 0), 4f,
            Time.deltaTime);
    }

    void ShootPlayer()
    {
        _health.Damage(gunDamage, _player.transform.position - eyeTransform.position);
        _player.GetComponent<CameraKickController>().AddKick(Quaternion.Euler(-5,0,3));
    }
    
    void OnShoot(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit)
    {
        // flinch and shit
        animator.SetTrigger("flinch");
        health -= damage;
        if (health <= 0)
        {
            _dead = true;
            TryDisengage();
            GetComponent<Collider>().enabled = false;
            animator.SetTrigger("death");
            Destroy(gameObject);
        }
    }
}
