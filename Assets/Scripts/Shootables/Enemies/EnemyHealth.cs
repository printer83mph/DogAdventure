using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    // delegate shit
    public delegate void OnDeath();
    public OnDeath onDeath = delegate { };
    public delegate void OnBulletDeath(PlayerShotInfo info);
    public OnBulletDeath onBulletDeath = delegate { };

    public float Health => _health;
    public bool Dead => _dead;

    // inspector
    [SerializeField]
    private float _health = 15;

    // auto-assigned
    private Shootable _shootable;
    
    // mathstuff
    private bool _dead;

    private void Awake() {
        _shootable = GetComponent<Shootable>();
    }

    private void OnEnable() {
        if (_shootable) _shootable.onShot += OnShot;
    }

    private void OnDisable() {
        if (_shootable) _shootable.onShot -= OnShot;
    }

    private void OnShot(PlayerShotInfo info) {
        if (_dead) return;
        Damage(info.damage);
        if (_dead) onBulletDeath(info);
    }
    
    public void Damage(float amt) {
        if (_dead) return;
        _health -= amt;
        if (_health <= 0) {
            Debug.Log("Bro.. i died");
            _dead = true;
            onDeath();
        }
    }

}