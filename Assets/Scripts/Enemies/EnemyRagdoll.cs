using Stims;
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
    // private EnemyHealth _health;

    private void Awake() {
        _rbs = GetComponentsInChildren<Rigidbody>();
        _cols = GetComponentsInChildren<Collider>();
        _animator = GetComponent<Animator>();
        _mainCollider = GetComponent<Collider>();
        // _health = GetComponent<EnemyHealth>();
        // make ourselves disable on death
    }

    private void Start() {
        SetRagdoll(_enabled);
    }

    private void OnDeath(Stim stim)
    {
        if (ragdollInstantlyOnDeath) SetRagdoll(true);
        switch (stim)
        {
            case HitscanDamageStim hitscanStim:
                OnBulletDeath(hitscanStim);
                break;
            case Stim.Katana katanaStim:
                OnKatanaDeath(katanaStim);
                break;
            case SourcedPointForceStim forceStim:
                OnBluntForceDeath(forceStim);
                break;
        }
    }

    private void OnBluntForceDeath(SourcedPointForceStim stim)
    {
        Vector3 baseForce = stim.Force();
        Rigidbody closest = _rbs[0];
        float dist = 1f;
        foreach (Rigidbody rb in _rbs)
        {
            float newDist = rb.ClosestPointOnBounds(stim.Point()).sqrMagnitude;
            if (newDist < dist)
            {
                closest = rb;
                dist = newDist;
            }
        }

        closest.AddForceAtPosition(baseForce, stim.Point());
    }

    private void OnBulletDeath(HitscanDamageStim stim)
    {
        Vector3 baseForce = stim.Force();
        Rigidbody closestRb = _rbs[0];
        Vector3 closestPoint = transform.position;
        float closestDistance = 1000;
        foreach(Rigidbody rb in _rbs)
        {
            Vector3 point = rb.ClosestPointOnBounds(stim.Point());
            float dist = Vector3.SqrMagnitude(point - stim.Point());
            if (dist < closestDistance)
            {
                closestRb = rb;
                closestPoint = point;
                closestDistance = dist;
            }
        }
        closestRb.AddForceAtPosition(baseForce, closestPoint, ForceMode.Impulse);
    }

    private void OnKatanaDeath(Stim.Katana stim)
    {
        Vector3 baseForce = stim.damager.transform.forward * -katanaDeathForce;
        Rigidbody closestRB = _rbs[0];
        float closestDistance = 10000;
        foreach(Rigidbody rb in _rbs)
        {
            float newDist = Vector3.SqrMagnitude(rb.position - stim.damager.transform.position);
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