using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{

    // inspector vars
    public string enemyType;
    public float attackingDistance = 10f;
    public float turnSpeed = 150f;
    public bool locked;

    // for other scripts
    public bool CanAttack => _canAttack;
    public bool CanSeePlayer => _vision.CanSeePlayer;
    public float PlayerDistance => _vision.PlayerDistance;

    // auto-assigned
    private NavMeshAgent _agent;
    private EnemyVision _vision;
    private PlayerController _player;
    private ChadistAI _chadistAI;

    // math stuff
    private bool _engaging;
    private bool _canAttack;

    void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<EnemyVision>();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _chadistAI = GameObject.FindGameObjectWithTag("Chadist AI").GetComponent<ChadistAI>();
    }

    void OnEnable() {
        _chadistAI.onSpotDelegate += OnSpot;
        _chadistAI.enemyBehaviours.Add(this);
    }

    void OnDisable() {
        _chadistAI.onSpotDelegate -= OnSpot;
        _chadistAI.enemyBehaviours.Remove(this);
    }

    void OnSpot() {
        if (!_agent.enabled) return;
        _agent.destination = _chadistAI.lastKnownPos;
    }

    public Vector3 VecToPlayer() {
        return _player.transform.position - transform.position;
    }

    void Update() {
        _canAttack = false;
        if (_vision.CanSeePlayer) {
            // we have LOS with the player!
            _chadistAI.SpotPlayer(_player.transform.position);
        }
        // break from this if we're not on alert at all
        if (_chadistAI.alertStatus == 0) return;
        if (locked) return;
        if (_engaging) {
            _canAttack = (_vision.CanSeePlayer && VecToPlayer().magnitude < attackingDistance);
            _agent.isStopped = _canAttack;
        } else {
            RotateTowards(_chadistAI.lastKnownPos, turnSpeed);
        }
    }

    private void RotateTowards(Vector3 destination, float turnSpeed) {
        Vector3 vecToDestination = destination - transform.position;
        vecToDestination.y = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vecToDestination), turnSpeed * Time.deltaTime);
    }

    public void SetEngaging(bool engaging) {
        _agent.enabled = engaging;
        _engaging = engaging;
    }
    
}