using UnityEngine;

public class EnemyVision : MonoBehaviour {

    // delegate stuff
    public delegate void VisionEvent(bool canSeePlayer);
    public VisionEvent onVisionUpdate = delegate { };
    public VisionEvent onCapsuleVisionUpdate = delegate { };

    // inspector vars
    public Transform eyeTransform;

    public LayerMask layerMask = (1 << 0) | (1 << 9);

    public float maxDistance = 25;
    public float maxAngle = 85;
    // TODO: make los radius only affect whether or not we can attack yet
    public float losRadius = 0;

    // math stuff
    private float _playerDistance;
    private bool _canSeePlayer;
    private bool _canCapsulePlayer;
    public float PlayerDistance => _playerDistance;
    public bool CanSeePlayer => _canSeePlayer;
    public bool CanCapsulePlayer => _canCapsulePlayer;

    // auto-assigned
    private PlayerController _player;
    private CapsuleCollider _collider;
    private EnemyHealth _health;
    private Transform _playerCam;
    private Vector3 _vecToPlayer;

    void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<EnemyHealth>();
        
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerCam = _player.GetComponentInChildren<Camera>().transform;
    }

    private void OnEnable()
    {
        if (_health) _health.onDeath += OnDeath;
    }
    
    private void OnDisable()
    {
        if (_health) _health.onDeath -= OnDeath;
    }

    private void OnDeath()
    {
        this.enabled = false;
    }

    void Update()
    {
        _vecToPlayer = _playerCam.transform.position - eyeTransform.position;
        _playerDistance = _vecToPlayer.magnitude;
        
        bool canSeePlayer = false;
        bool canCapsulePlayer = false;
        if (Vector3.Angle(_vecToPlayer, eyeTransform.forward) > maxAngle || _playerDistance > maxDistance) {
            // player not even in cone of vision
        } else {
            // player is in cone of vision
            // Debug.DrawRay(eyeTransform.position, _vecToPlayer);
            canSeePlayer = (!Physics.Raycast(eyeTransform.position, _vecToPlayer, out RaycastHit hit, _playerDistance, layerMask));
            if (losRadius == 0) {
                canCapsulePlayer = canSeePlayer;
            } else {
                Vector3 start = Vector3.MoveTowards(eyeTransform.position, _playerCam.transform.position, losRadius);
                Vector3 end = Vector3.MoveTowards(_playerCam.transform.position, eyeTransform.position, losRadius);
                // Debug.DrawRay(start, end - start);
                _collider.enabled = false;
                canCapsulePlayer = (!Physics.CheckCapsule(start, end, losRadius, layerMask));
                _collider.enabled = true;
            }
        }
        
        if (canSeePlayer != _canSeePlayer) {
            onVisionUpdate(canSeePlayer);
            _canSeePlayer = canSeePlayer;
            Debug.Log("Vision update");
        }

        if (canCapsulePlayer != _canCapsulePlayer) {
            onCapsuleVisionUpdate(canCapsulePlayer);
            _canCapsulePlayer = canCapsulePlayer;
        }

    }

}