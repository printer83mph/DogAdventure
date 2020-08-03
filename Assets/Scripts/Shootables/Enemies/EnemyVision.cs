using UnityEngine;

public class EnemyVision : MonoBehaviour {

    // delegate stuff
    // public delegate void OnSpotDelegate(bool canSeePlayer);
    // public OnSpotDelegate onSpotDelegate;

    // inspector vars
    public Transform eyeTransform;

    public LayerMask layerMask = (1 << 0) | (1 << 9);

    public float maxDistance = 25;
    public float maxAngle = 85;

    // math stuff
    private float _playerDistance;
    private bool _canSeePlayer;
    public float PlayerDistance => _playerDistance;
    public bool CanSeePlayer => _canSeePlayer;

    // auto-assigned
    private PlayerController _player;
    private Transform _playerCam;
    private Vector3 _vecToPlayer;

    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerCam = _player.GetComponentInChildren<Camera>().transform;
    }

    void Update() {

        _playerDistance = -1;
        _vecToPlayer = _playerCam.transform.position - eyeTransform.position;
        _playerDistance = _vecToPlayer.magnitude;
        if (Vector3.Angle(_vecToPlayer, eyeTransform.forward) > maxAngle || _playerDistance > maxDistance) return;
        Debug.DrawRay(eyeTransform.position, _vecToPlayer);
        // this is so cool
        bool canSeePlayer = (!Physics.Raycast(eyeTransform.position, _vecToPlayer, out RaycastHit hit, _playerDistance, layerMask));
        if (canSeePlayer != _canSeePlayer) {
            _canSeePlayer = canSeePlayer;
            // onSpotDelegate(canSeePlayer);
        }
    }

}