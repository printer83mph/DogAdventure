using UnityEngine;

[RequireComponent(typeof(Shootable))]
public class BreakableProp : MonoBehaviour
{
    
    public float health;
    public GameObject fxPrefab;

    void Start()
    {
        GetComponent<Shootable>().onShootDelegate += OnShoot;
    }
    
    public void OnShoot(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit)
    {
        health -= damage;
        if (health <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        GameObject fx = Instantiate(fxPrefab);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }

}
