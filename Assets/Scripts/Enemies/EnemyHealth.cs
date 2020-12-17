using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    // delegate shit
    public delegate void OnDeathEvent(Damage damage);
    public OnDeathEvent onDeath = delegate { };

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
        }
    }

    private void OnDamage(Damage damage) {
        if (_dead) return;
        Damage(damage);
    }

    public void Damage(Damage damage) {
        if (_dead) return;
        _health -= damage.damage;
        if (_health <= 0) {
            Debug.Log("Bro.. i died");
            _dead = true;
            onDeath(damage);
        }
    }

}