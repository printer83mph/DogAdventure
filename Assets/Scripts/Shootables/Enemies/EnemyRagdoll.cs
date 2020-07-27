using UnityEngine;

public class EnemyRagdoll : MonoBehaviour {

  public Rigidbody hitRB;
  public float deathForceScale = 100000;
  public float invSquareDistanceScale = .01f;
  
  [SerializeField]
  private bool _startEnabled;
  private Rigidbody[] _rbs;
  private Collider[] _cols;
  private Animator _animator;
  private Collider _mainCollider;

  private void Start() {
    _rbs = GetComponentsInChildren<Rigidbody>();
    _cols = GetComponentsInChildren<Collider>();
    _animator = GetComponent<Animator>();
    _mainCollider = GetComponent<Collider>();
    SetRagdoll(_startEnabled);
  }

  public void HitDeath(PlayerShotInfo info) {
    SetRagdoll(true);
    Vector3 baseForce = info.direction * (deathForceScale * info.damage);
    foreach(Rigidbody rb in _rbs) {
      Vector3 closestPoint = rb.GetComponent<Collider>().ClosestPoint(info.hit.point);
      float invSquare = Vector3.SqrMagnitude((closestPoint - info.hit.point) * invSquareDistanceScale) + .02f;
      rb.AddForceAtPosition(baseForce / invSquare, closestPoint);
    }
  }

  public void SetRagdoll(bool on) {
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