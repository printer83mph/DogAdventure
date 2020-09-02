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
    public string enemyType;
    public float attackingDistance = 10f;
    public float turnSpeed = 150f;
    public bool turnTowardsPlayer = true;

    // for other scripts
    public bool CanAttack => _canAttack;
    public bool Locked => _locked;
    public bool CanSeePlayer => _vision.CanSeePlayer;
    public float PlayerDistance => _vision.PlayerDistance;
    public Vector3 AgentVelocity => _agent.velocity;
    public NavMeshAgent Agent => _agent;

    // auto-assigned
    private NavMeshAgent _agent;
    private EnemyVision _vision;
    private PlayerController _player;
    private ChadistAI _chadistAI;
    private EnemyHealth _health;

    // math stuff
    private bool _engaging;
    private bool _locked;
    private bool _canAttack;

    void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<EnemyVision>();
        _health = GetComponent<EnemyHealth>();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void OnEnable() {
        _chadistAI.onSpotDelegate += OnSpot;
        if (_health) _health.onDeath += OnDeath;
        _chadistAI.enemyBehaviours.Add(this);
    }

    void OnDisable() {
        _chadistAI.onSpotDelegate -= OnSpot;
        if (_health) _health.onDeath -= OnDeath;
        _chadistAI.enemyBehaviours.Remove(this);
    }

    void OnSpot() {
        if (!_agent.enabled) return;
    }

    public Vector3 VecToPlayer() {
        return _player.transform.position - transform.position;
    }

    public Vector3 VecToLastKnownPos() {
        return _chadistAI.lastKnownPos - transform.position;
    }

    void Update() {
        bool canAttack = false;
        if (_vision.CanSeePlayer) {
            // we have LOS with the player!
            _chadistAI.SpotPlayer(_player.transform.position);
        }
        // break from this if we're not on alert at all
        if (_chadistAI.alertStatus != 0)
        {
            if (_engaging)
            {
                if (_agent.enabled) _agent.destination = _chadistAI.lastKnownPos;
                // find out if we're close enough to stop
                canAttack = (_vision.CanSeePlayer && VecToPlayer().magnitude < attackingDistance);
                if (_agent.enabled) _agent.isStopped = canAttack;
            }
            if (_agent.enabled && _agent.isStopped && turnTowardsPlayer)
            {
                RotateTowards(_chadistAI.lastKnownPos, turnSpeed);
            }
        }
        
        if (canAttack != _canAttack) {
            _canAttack = canAttack;
            onAttackUpdate(_canAttack);
        }
    }

    private void RotateTowards(Vector3 destination, float turnSpeed) {
        Vector3 vecToDestination = destination - transform.position;
        vecToDestination.y = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vecToDestination), turnSpeed * Time.deltaTime);
    }

    // for our big ai controller to manage
    public void SetEngaging(bool engaging) {
        _engaging = engaging;
        _agent.enabled = _engaging && !_locked;
        onEngageUpdate(_engaging);
    }

    // for individual AI to manage (charging and shit)
    public void SetLocked(bool locked) {
        _locked = locked;
        _agent.enabled = _engaging && !_locked;
    }

    public void OnDeath() {
        _agent.enabled = false;
        this.enabled = false;
    }
    
}