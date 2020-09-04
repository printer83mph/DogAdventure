using UnityEngine;

public class EnemyRagdoll : MonoBehaviour {

    public bool ragdollInstantlyOnDeath = true;
    public Rigidbody hitRB;
    public float deathForceScale = 100000;
    public float invSquareDistanceScale = .01f;

    [SerializeField]
    private bool _enabled;

    // auto-assigned
    private Rigidbody[] _rbs;
    private Collider[] _cols;
    private Animator _animator;
    private Collider _mainCollider;
    private EnemyHealth _health;

    private void Awake() {
        _rbs = GetComponentsInChildren<Rigidbody>();
        _cols = GetComponentsInChildren<Collider>();
        _animator = GetComponent<Animator>();
        _mainCollider = GetComponent<Collider>();
        _health = GetComponent<EnemyHealth>();
    }

    private void OnEnable() {
        if (_health) {
            _health.onDeath += OnDeath;
            _health.onBulletDeath += OnBulletDeath;
        }
    }

    private void OnDisable() {
        if (_health) {
            _health.onDeath -= OnDeath;
            _health.onBulletDeath -= OnBulletDeath;
        }
    }

    private void Start() {
        SetRagdoll(_enabled);
    }

    private void OnBulletDeath(PlayerShotInfo info) {
        SetRagdoll(true);
        Vector3 baseForce = info.direction * (deathForceScale * info.damage);
        foreach(Rigidbody rb in _rbs) {
            Vector3 closestPoint = rb.GetComponent<Collider>().ClosestPoint(info.hit.point);
            float invSquare = Vector3.SqrMagnitude((closestPoint - info.hit.point) * invSquareDistanceScale) + .02f;
            // rb.AddForceAtPosition(baseForce / invSquare, closestPoint);
            rb.AddForceAtPosition(baseForce / invSquare, info.hit.point);
        }
    }

    private void OnDeath() {
        if (ragdollInstantlyOnDeath) SetRagdoll(true);
    }

    public void SetRagdoll(bool on) {
        _enabled = on;

        foreach (Rigidbody rb in _rbs) {
            rb.isKinematic = !on;
        }
        foreach (Collider col in _cols) {
            col.enabled = on;
        }

        _animator.enabled = !on;
        _mainCollider.enabled = !on;

    }

}