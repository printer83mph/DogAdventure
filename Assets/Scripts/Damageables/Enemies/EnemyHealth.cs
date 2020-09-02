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
    private Damageable _damageable;
    
    // mathstuff
    private bool _dead;

    private void Awake() {
        _damageable = GetComponent<Damageable>();
        
        if (_damageable) {
            _damageable.onDamage += OnDamage;
            _damageable.onShot += OnShot;
        }
    }

    private void OnDamage(float damage) {
        if (_dead) return;
        Damage(damage);
    }

    private void OnShot(PlayerShotInfo info) {
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