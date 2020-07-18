using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Shootable))]
[RequireComponent(typeof(NavMeshAgent))]
public class SecurityEnemy : MonoBehaviour
{

    public Animator animator;
    public Transform eyeTransform;

    public float maxShootDistance = 20f;
    public float walkSpeedAnimScale = .2f;

    public float health = 10;

    public LayerMask layerMask;
    
    private NavMeshAgent _agent;
    private Transform _player;
    private PlayerHealth _health;
    private bool _los;
    private bool _dead;

    private static List<SecurityEnemy> _securityEnemies = new List<SecurityEnemy>();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Shootable>().onShootDelegate += OnShoot;
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        _health = _player.GetComponent<PlayerHealth>();
        _securityEnemies.Add(this);
    }

    private void OnDestroy()
    {
        _securityEnemies.Remove(this);
    }

    private void Update()
    {
        if (_dead) return;
        
        // base layer info
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        
        _agent.SetDestination(_player.position);
        
        bool newLos = GetLOS();
        if (_los != newLos)
        {
            animator.SetBool("lineOfSight", newLos);
            Debug.Log("Locked/unlocked to player");
        }
        
        if (info.IsName("Base"))
        {
            // we're running around until we get line of sight
            _agent.isStopped = false;
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
            if (info.IsName("Shooting"))
            {
                Debug.Log("You got shot you fuckin idiot");
            }
        }
        _los = newLos;
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

    bool GetLOS()
    {
        Ray toPly = new Ray(eyeTransform.position, _player.position - eyeTransform.position);
        if (Physics.Raycast(toPly, out RaycastHit hit, maxShootDistance, layerMask))
        {
            if (hit.transform == _player) return true;
        }
        return false;
    }

    void ShootPlayer()
    {
        _health.Damage(.5f, _player.position - eyeTransform.position);
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
