using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class BreakableProp : MonoBehaviour
{
    
    public float health;
    public GameObject fxPrefab;
    
    private Damageable _damageable;

    void Awake() {
        _damageable = GetComponent<Damageable>();
    }

    void OnEnable() {
        _damageable.onDamage += OnShoot;
    }

    void OnDisable() {
        _damageable.onDamage -= OnShoot;
    }
    
    public void OnShoot(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        if (fxPrefab) {
            GameObject fx = Instantiate(fxPrefab);
            fx.transform.position = transform.position;
        }
        Destroy(gameObject);
    }

}
