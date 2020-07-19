using UnityEngine;

public class EnemyVision : MonoBehaviour {

    public Transform eyeTransform;

    public LayerMask layerMask;

    public float maxDistance = 25;
    public float maxAngle = 70;

    private float _playerLOSDistance;
    public float PlayerLOSDistance => _playerLOSDistance;
    public bool CanSeePlayer => _playerLOSDistance != -1;

    // auto-assigned
    private PlayerController _player;
    private Transform _playerCam;

    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerCam = _player.GetComponentInChildren<Camera>().transform;
    }

    void Update() {

        _playerLOSDistance = -1;
        Ray toPly = new Ray(eyeTransform.position, _playerCam.transform.position - eyeTransform.position);
        if (Vector3.Angle(toPly.direction, eyeTransform.forward) > maxAngle) return;
        if (Physics.Raycast(toPly, out RaycastHit hit, maxDistance, layerMask))
        {
            if (hit.transform == _player.transform) {
                _playerLOSDistance = hit.distance;
            }
        }
    }

}