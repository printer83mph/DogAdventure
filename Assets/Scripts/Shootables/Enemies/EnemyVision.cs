using UnityEngine;

public class EnemyVision : MonoBehaviour {

    public Transform eyeTransform;

    public LayerMask layerMask;

    public float maxDistance;
    public float maxAngle;

    public float playerLOSDistance;
    private Transform _playerCam;

    void Start() {
        _playerCam = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>().transform;
    }

    void Update() {

        // TODO: this
        Ray toPly = new Ray(eyeTransform.position, _playerCam.transform.position - eyeTransform.position);
        if (Physics.Raycast(toPly, out RaycastHit hit, maxDistance, layerMask))
        {
            if (hit.transform == _playerCam) {
                return hit.distance;
            }
        }

        return 0;
    }

}