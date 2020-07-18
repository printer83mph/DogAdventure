using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Shootable))]
[RequireComponent(typeof(NavMeshAgent))]
public class SecurityEnemy : MonoBehaviour
{

    public Animator animator;
    public Transform eyeTransform;

    public float maxShootDistance = 7f;
    public float maxVisionDistance = 20f;
    public float walkRunScaler = .2f;

    public float health = 10;

    public LayerMask layerMask;
    
    // auto-assigned
    private NavMeshAgent _agent;
    private Transform _player;
    private PlayerHealth _health;
    private ChadistAI _chadistAI;

    // math stuff
    private bool _hasLos;
    private bool _inShootingRange;
    private bool _dead;
    private bool _engaging;

    static int numEngagingPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Shootable>().onShootDelegate += OnShoot;
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        _health = _player.GetComponent<PlayerHealth>();
        _chadistAI = GameObject.FindWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    private void Update()
    {
        if (_dead) return;
        
        // base layer info
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        
        _agent.SetDestination(_chadistAI.lastKnownPos);
        
        // update line of sight and spotting data
        bool newInShootingRange = GetWithinShootDistance();

        // update chadist AI if player is spotted
        if (_hasLos) {
            _chadistAI.SpotPlayer(_player.transform.position);
        }

        if (newInShootingRange) {
            // we are able to shoot the player, but do we?
            if (!_engaging && numEngagingPlayer < _chadistAI.maxSecurityEngaging) {
                //engage
                _engaging = true;
                numEngagingPlayer ++;
                animator.SetBool("canShoot", true);
            }
            _agent.isStopped = true;
        } else {
            // we're not in range so we need to get there
            // disengage if we were engaged
            if (_engaging) {
                _engaging = false;
                numEngagingPlayer --;
                animator.SetBool("canShoot", false);
            }
            // dont move if we already have max engaging
            _agent.isStopped = numEngagingPlayer == _chadistAI.maxSecurityEngaging;
            _engaging = false;
        }
        
        if (info.IsName("Base"))
        {
            // we're running around until we are in shooting distance
            if (_chadistAI.alertStatus == 0)
            {
                _agent.isStopped = true;
            }
            // TODO: wander around if on alert but position not known
            // TODO: limit the number of security enemies confronting player
            Vector3 vel = transform.InverseTransformVector(_agent.velocity) * (1/_agent.speed);
            animator.SetFloat("xVel", vel.x);
            animator.SetFloat("zVel", vel.z);
        }
        // if not flinched rotate towards player
        if (!info.IsName("Flinch") && _agent.isStopped)
        {
            RotateTowardsPlayer();
        }
        _inShootingRange = newInShootingRange;
    }

    void RotateTowardsPlayer()
    {
        Vector2 xzPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 plyxzPos = new Vector2(_player.position.x, _player.position.z);
        transform.rotation = PrintUtil.Damp(transform.rotation,
            Quaternion.Euler(0,
                Quaternion.LookRotation(_player.position - transform.position, Vector3.up).eulerAngles.y, 0), 10f,
            Time.deltaTime);
    }

    bool GetWithinShootDistance()
    {
        Ray toPly = new Ray(eyeTransform.position, _player.position - eyeTransform.position);
        if (Physics.Raycast(toPly, out RaycastHit hit, maxVisionDistance, layerMask))
        {
            if (hit.transform == _player) {
                _hasLos = true;
                if (hit.distance < maxShootDistance) {
                    return true;
                } else return false;
            } else {
                _hasLos = false;
            }
        }
        return false;
    }

    void ShootPlayer()
    {
        _health.Damage(.5f, _player.position - eyeTransform.position);
        _player.GetComponent<CameraKickController>().AddKick(Quaternion.Euler(-5,0,3));
        Debug.Log("You got shot you fuckin idiot");
    }
    
    void OnShoot(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit)
    {
        // flinch and shit
        animator.SetTrigger("flinch");
        health -= damage;
        if (health <= 0)
        {
            _dead = true;
            if (_engaging) {
                numEngagingPlayer --;
            }
            GetComponent<Collider>().enabled = false;
            animator.SetTrigger("death");
            Destroy(gameObject);
        }
    }
}
