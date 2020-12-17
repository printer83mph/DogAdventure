using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{

    // fun events
    public delegate void BehaviourUpdate(bool newState);
    public BehaviourUpdate onEngageUpdate = delegate { };
    public BehaviourUpdate onAttackUpdate = delegate { };

    // inspector vars
    public float attackingDistance = 10f;
    public float turnSpeed = 150f;
    public bool turnTowardsPlayer = true;

    // for other scripts
    public bool Locked => _locked;
    public bool CanSeePlayer => _vision.CanSeePlayer;
    public bool CanAttackPlayer => _vision.CanCapsulePlayer;
    public float PlayerDistance => _vision.PlayerDistance;
    public float LastKnownPosDistance => VecToLastKnownPos.magnitude;
    public Vector3 AgentVelocity => _agent.velocity;
    public NavMeshAgent Agent => _agent;
    [SerializeField]
    private Squad _squad;
    public Squad Squad => _squad;
    // todo: squad generator, transports

    // auto-assigned
    private NavMeshAgent _agent;
    private EnemyVision _vision;
    private Damageable _damageable;
    private PlayerController _player;
    private ChadistAI _chadistAI;
    private EnemyHealth _health;

    // math stuff
    private bool _engaging; // used for whether or not we're engaging
    private bool _attacking; // used to tell other systems if we should be attacking
    public bool Attacking => _attacking;
    private bool _locked; // used by other systems for stopping our movement
    private bool _overrideEngageLimit; // becomes true when hand is forced
    public bool OverrideEngageLimit => _overrideEngageLimit;
    
    private Vector3 _positionOfInterest; // used to track player and whatnot
    private float _horniness;
    private bool _horny;

    // util methods
    public Vector3 VecToPlayer => _player.transform.position - transform.position;
    public Vector3 VecToLastKnownPos => _squad.lastKnownPos - transform.position;
    public Vector3 VecToPosOfInterest => _positionOfInterest - transform.position;

    public bool WithinAttackingDistance => (_player.transform.position - transform.position).sqrMagnitude <
                                           Mathf.Pow(attackingDistance, 2);
    
    void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<EnemyVision>();
        _damageable = GetComponent<Damageable>();
        _health = GetComponent<EnemyHealth>();

        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        // _chadistAI.onSpotDelegate += OnSpot;
        // _chadistAI.enemyBehaviours.Add(this);
        if (_health) _health.onDeath += OnDeath;
        if (_damageable) _damageable.onDamage += OnDamage;

        if (!_chadistAI.enemies.Contains(this)) _chadistAI.enemies.Add(this);
    }

    private void OnDisable()
    {
        // _chadistAI.onSpotDelegate -= OnSpot;
        // _chadistAI.enemyBehaviours.Remove(this);
        
        // might break stuff
        if (_health) _health.onDeath -= OnDeath;
        if (_damageable) _damageable.onDamage -= OnDamage;
        
        if (_chadistAI.enemies.Contains(this)) _chadistAI.enemies.Remove(this);
    }

    private void Start()
    {
        _positionOfInterest = transform.position;
    }

    // squad specific methods ------
    
    public void SetSquad(Squad squad)
    {
        _squad = squad;
        // squad updating logic
    }

    public void ClearSquad()
    {
        _squad = null;
        // squad clearing logic
    }

    void Update()
    {
        UpdateVision();
        UpdateEngagement();
        UpdateMovement();
        if (_squad != null)
        {
            // we don't have a squad
            
        }
    }

    private void UpdateVision()
    {
        // add horniness if we can see the player
        if (_vision.CanSeePlayer)
        {
            _horniness = Mathf.MoveTowards(_horniness, 1, Time.deltaTime * 1f);
            if (_horny)
            {
                // horny and we see the player
                _squad?.Alert(_player.transform.position);
                _positionOfInterest = _player.transform.position;
            }
        }
        else
        {
            _horniness = Mathf.MoveTowards(_horniness, 0, Time.deltaTime * .25f);
        }
        
        if (!_horny)
        {
            if (_horniness == 1f)
            {
                _horny = true;
                Debug.Log("Now Horny");
            }
        }
        else
        {
            if (_horniness == 0) _horny = false;
        }

    }

    private void UpdateEngagement()
    {
        // return if we're already at max engaging or overriding it
        if (_overrideEngageLimit) return;

        if (!_engaging)
        {
            if (_horny) // && _vision.CanSeePlayer ?
            {
                TryEngage();
            }

            if (_squad != null)
            {
                if (_squad.AlertStatus == AlertStatus.FullAlert) TryEngage();
            }
        }
        else
        {
            SetAttacking(_vision.CanCapsulePlayer && WithinAttackingDistance);
            
            // if not horny and last alert was a while ago then just disengage
        }
    }

    private void SetAttacking(bool attacking)
    {
        if (_attacking == attacking) return;

        _attacking = attacking;
        onAttackUpdate(attacking);
    }

    public void ForceEngage()
    {
        _overrideEngageLimit = true;
        _engaging = true;
        onEngageUpdate(true);
        StartCoroutine(nameof(ForceEngageLoop));
    }

    IEnumerator ForceEngageLoop()
    {
        
        // if can't see player for 1 second then undo override
        do
        {
            yield return new WaitForSeconds(1);
        } while (CanSeePlayer);

        _engaging = false;
        onEngageUpdate(false);
        _overrideEngageLimit = false;
    }

    public void TryEngage()
    {
        if (_chadistAI.MaxEngaging || _engaging) return;

        Debug.Log("Engaging.");
        // actually engage
        _engaging = true;
        onEngageUpdate(true);
        _chadistAI.engaging.Add(this);
    }

    public void Disengage()
    {
        if (!_engaging) return;
        
        // actually disengage
        _engaging = false;
        onEngageUpdate(false);
        _chadistAI.engaging.Remove(this);
    }

    private void UpdateMovement()
    {
        bool stopped = _locked || (WithinAttackingDistance && _vision.CanCapsulePlayer);
        if (!(stopped && !turnTowardsPlayer)) RotateTowards(_positionOfInterest);
        
        _agent.enabled = !_locked;
        if (_locked) return;
        
        // go to position of interest
        _agent.destination = _positionOfInterest;
        // stop if not engaging or if locked
        _agent.isStopped = stopped;
    }

    private void RotateTowards(Vector3 destination) {
        Vector3 vecToDestination = destination - transform.position;
        vecToDestination.y = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vecToDestination),
            turnSpeed * Time.deltaTime);
    }

    // for other systems to manage
    public void SetLocked(bool locked)
    {
        if (locked == _locked) return;
        _locked = locked;
    }

    private void OnDamage(Damage damage)
    {
        // special damage things
        if (damage is Damage.SourcedDamage)
        {
            if (((Damage.SourcedDamage) damage).source == GenericDamageSources.Player && CanSeePlayer)
            {
                ForceEngage();
                _horniness = 1f;
                _horny = true;
            }
        }
    }

    private void OnDeath(Damage damage) {
        _agent.enabled = false;
        enabled = false;
    }


    public void SoundAlert(Vector3 pos, SoundType soundType)
    {
        // todo: investigative behaviour
        if (soundType == SoundType.Alarming)
        {
            _squad?.Alert(pos);
        }

        if (soundType == SoundType.Alarming || soundType == SoundType.Suspicious)
        {
            Debug.Log("Suspicious or alarming sound heard");
            _positionOfInterest = pos;
            _horny = true;
            _horniness = 1;
        }
    }
}