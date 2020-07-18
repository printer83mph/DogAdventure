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
    public float walkSpeedAnimScale = .2f;

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
        
        bool newInShootingRange = GetWithinShootDistance();

        // update chadist AI if player is spotted
        if (_hasLos) {
            _chadistAI.SpotPlayer(_player.transform.position);
        }

        // update animator if we have to
        if (_inShootingRange != newInShootingRange)
        {
            animator.SetBool("lineOfSight", newInShootingRange);
        }
        
        if (info.IsName("Base"))
        {
            // we're running around until we are in shooting distance
            _agent.isStopped = _chadistAI.alertStatus == 0;
            animator.SetFloat("walkSpeed", _agent.velocity.magnitude * walkSpeedAnimScale);
        }
        else
        {
            // if not flinched rotate towards player
            if (!info.IsName("Flinch"))
            {
                RotateTowardsPlayer();
            }
            _agent.isStopped = true;
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
            GetComponent<Collider>().enabled = false;
            animator.SetTrigger("death");
            Destroy(gameObject);
        }
    }
}
