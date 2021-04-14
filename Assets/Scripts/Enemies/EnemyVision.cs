using System;
using Player.Controlling;
using ScriptableObjects.Enemies;
using Stims;
using UnityEngine;

public class EnemyVision : MonoBehaviour {

    // inspector vars

    [SerializeField] private EnemyVisionConfig visionConfig = null;
    [SerializeField] private CapsuleCollider enemyCollider = null;

    // math stuff
    private float _playerDistance;
    private bool _canSeePlayer;
    private bool _canCapsulePlayer;
    public float PlayerDistance => _playerDistance;
    public bool CanSeePlayer => _canSeePlayer;
    public bool CanCapsulePlayer => _canCapsulePlayer;

    // auto-assigned
    private Transform _playerCam;
    private Vector3 _vecToPlayer;

    private void Awake()
    {
        // make ourselves disable on death
    }

    private void Start()
    {
        _playerCam = PlayerController.Main.Orientation;
    }

    private void OnDeath(Stim stim)
    {
        enabled = false;
    }

    private void Update()
    {
        _vecToPlayer = _playerCam.transform.position - transform.position;
        _playerDistance = _vecToPlayer.magnitude;
        
        UpdateVision();
        
    }
    
    private void UpdateVision()
    {
        _canSeePlayer = false;
        _canCapsulePlayer = false;
        if (Vector3.Angle(_vecToPlayer, transform.forward) > visionConfig.MaxAngle || _playerDistance > visionConfig.MaxDistance)
        {
            // player not even in cone of vision
        }
        else
        {
            // player is in cone of vision
            // Debug.DrawRay(eyeTransform.position, _vecToPlayer);
            _canSeePlayer = (!Physics.Raycast(transform.position, _vecToPlayer, out RaycastHit hit, _playerDistance,
                visionConfig.LayerMask));
            if (visionConfig.LosRadius == 0)
            {
                _canCapsulePlayer = _canSeePlayer;
            }
            else
            {
                Vector3 start = Vector3.MoveTowards(transform.position, _playerCam.transform.position, visionConfig.LosRadius);
                Vector3 end = Vector3.MoveTowards(_playerCam.transform.position, transform.position, visionConfig.LosRadius);
                // Debug.DrawRay(start, end - start);
                enemyCollider.enabled = false;
                _canCapsulePlayer = (!Physics.CheckCapsule(start, end, visionConfig.LosRadius, visionConfig.LayerMask));
                enemyCollider.enabled = true;
            }
        }
    }
}