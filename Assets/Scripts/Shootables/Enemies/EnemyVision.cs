using UnityEngine;

public class EnemyVision : MonoBehaviour {

    public Transform eyeTransform;

    public LayerMask layerMask = (1 << 0) | (1 << 9);

    public float maxDistance = 25;
    public float maxAngle = 85;

    private float _playerLOSDistance = -1;
    public float PlayerLOSDistance => _playerLOSDistance;
    public bool CanSeePlayer => _playerLOSDistance != -1;

    // auto-assigned
    private PlayerController _player;
    private Transform _playerCam;
    private Vector3 _vecToPlayer;

    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerCam = _player.GetComponentInChildren<Camera>().transform;
    }

    void Update() {

        _playerLOSDistance = -1;
        _vecToPlayer = _playerCam.transform.position - eyeTransform.position;
        if (Vector3.Angle(_vecToPlayer, eyeTransform.forward) > maxAngle || _vecToPlayer.sqrMagnitude > Mathf.Pow(maxDistance, 2)) return;
        Debug.DrawRay(eyeTransform.position, _vecToPlayer);
        if (!Physics.Raycast(eyeTransform.position, _vecToPlayer, out RaycastHit hit, _vecToPlayer.magnitude, layerMask))
        {
            _playerLOSDistance = hit.distance;
            Debug.Log("Can see player");
        }
    }

}