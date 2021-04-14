using Player.Controlling;
using ScriptableObjects.Enemies;
using Stims;
using UnityEngine;

public class EnemyVision : MonoBehaviour {

    // delegate stuff
    public delegate void VisionEvent(bool canSeePlayer);
    public VisionEvent onVisionUpdate = delegate { };
    public VisionEvent onCapsuleVisionUpdate = delegate { };

    // inspector vars
    [SerializeField] private Transform eyeTransform = null;

    [SerializeField] private EnemyVisionConfig visionConfig = null;

    // math stuff
    private float _playerDistance;
    private bool _canSeePlayer;
    private bool _canCapsulePlayer;
    public float PlayerDistance => _playerDistance;
    public bool CanSeePlayer => _canSeePlayer;
    public bool CanCapsulePlayer => _canCapsulePlayer;

    // auto-assigned
    private CapsuleCollider _collider;
    private Transform _playerCam;
    private Vector3 _vecToPlayer;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        // make ourselves disable on death
    }

    private void OnDeath(Stim stim)
    {
        enabled = false;
    }

    private void Update()
    {
        _vecToPlayer = _playerCam.transform.position - eyeTransform.position;
        _playerDistance = _vecToPlayer.magnitude;
        
        UpdateVision();
        
    }
    
    private void UpdateVision()
    {
        bool canSeePlayer = false;
        bool canCapsulePlayer = false;
        if (Vector3.Angle(_vecToPlayer, eyeTransform.forward) > visionConfig.MaxAngle || _playerDistance > visionConfig.MaxDistance)
        {
            // player not even in cone of vision
        }
        else
        {
            // player is in cone of vision
            // Debug.DrawRay(eyeTransform.position, _vecToPlayer);
            canSeePlayer = (!Physics.Raycast(eyeTransform.position, _vecToPlayer, out RaycastHit hit, _playerDistance,
                visionConfig.LayerMask));
            if (visionConfig.LosRadius == 0)
            {
                canCapsulePlayer = canSeePlayer;
            }
            else
            {
                Vector3 start = Vector3.MoveTowards(eyeTransform.position, _playerCam.transform.position, visionConfig.LosRadius);
                Vector3 end = Vector3.MoveTowards(_playerCam.transform.position, eyeTransform.position, visionConfig.LosRadius);
                // Debug.DrawRay(start, end - start);
                _collider.enabled = false;
                canCapsulePlayer = (!Physics.CheckCapsule(start, end, visionConfig.LosRadius, visionConfig.LayerMask));
                _collider.enabled = true;
            }
        }

        if (canSeePlayer != _canSeePlayer)
        {
            onVisionUpdate(canSeePlayer);
            _canSeePlayer = canSeePlayer;
        }

        if (canCapsulePlayer != _canCapsulePlayer)
        {
            onCapsuleVisionUpdate(canCapsulePlayer);
            _canCapsulePlayer = canCapsulePlayer;
        }
    }
}