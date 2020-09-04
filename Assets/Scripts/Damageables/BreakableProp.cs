using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class BreakableProp : MonoBehaviour
{

    public delegate void BreakEvent();
    public BreakEvent onBreak = delegate { };
    
    public float health;
    public GameObject fxPrefab;
    
    private Damageable _damageable;

    void Awake()
    {
        _damageable = GetComponent<Damageable>();
        _damageable.onDamage += OnShoot;
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
        onBreak();
        Destroy(gameObject);
    }

}
