using UnityEngine;

public class EnemyRagdoll : MonoBehaviour {

    public bool ragdollInstantlyOnDeath = true;
    public Rigidbody hitRB;
    public float katanaDeathForce = 100;
    public float deathForceScale = 15;

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
        }
    }

    private void OnDisable() {
        if (_health) {
            _health.onDeath -= OnDeath;
        }
    }

    private void Start() {
        SetRagdoll(_enabled);
    }

    private void OnDeath(Damage damage) {
        if (ragdollInstantlyOnDeath) SetRagdoll(true);
        if (damage is Damage.PlayerBulletDamage)
        {
            OnBulletDeath((Damage.PlayerBulletDamage) damage);
        }
        else if (damage is Damage.PlayerKatanaDamage)
        {
            OnKatanaDeath((Damage.PlayerKatanaDamage) damage);
        }
        else if (damage is Damage.BluntForceDamage)
        {
            OnBluntForceDeath((Damage.BluntForceDamage) damage);
        }
    }

    private void OnBluntForceDeath(Damage.BluntForceDamage damage)
    {
        Vector3 baseForce = damage.force.direction * deathForceScale;
        Rigidbody closest = _rbs[0];
        float dist = 1f;
        foreach (Rigidbody rb in _rbs)
        {
            float newDist = rb.ClosestPointOnBounds(damage.force.origin).sqrMagnitude;
            if (newDist < dist)
            {
                closest = rb;
                dist = newDist;
            }
        }

        closest.AddForceAtPosition(baseForce, damage.force.origin);
    }

    private void OnBulletDeath(Damage.PlayerBulletDamage damage) {
        Vector3 baseForce = damage.direction * (deathForceScale * damage.damage);
        Rigidbody closestRb = _rbs[0];
        Vector3 closestPoint = transform.position;
        float closestDistance = 1000;
        foreach(Rigidbody rb in _rbs)
        {
            Vector3 point = rb.ClosestPointOnBounds(damage.hit.point);
            float dist = Vector3.SqrMagnitude(point - damage.hit.point);
            if (dist < closestDistance)
            {
                closestRb = rb;
                closestPoint = point;
                closestDistance = dist;
            }
        }
        closestRb.AddForceAtPosition(baseForce, closestPoint, ForceMode.Impulse);
    }

    private void OnKatanaDeath(Damage.PlayerKatanaDamage damage)
    {
        Vector3 baseForce = damage.damager.transform.forward * -katanaDeathForce;
        Rigidbody closestRB = _rbs[0];
        float closestDistance = 10000;
        foreach(Rigidbody rb in _rbs)
        {
            float newDist = Vector3.SqrMagnitude(rb.position - damage.damager.transform.position);
            if (newDist < closestDistance)
            {
                closestDistance = newDist;
                closestRB = rb;
            }
        }
        closestRB.AddForce(baseForce, ForceMode.Impulse);
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