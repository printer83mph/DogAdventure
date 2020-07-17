using UnityEngine;

public class Shootable : MonoBehaviour
{

    public delegate void OnShootDelegate(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit);

    public OnShootDelegate onShootDelegate;

    void Start()
    {
        onShootDelegate += OnShoot;
    }
    
    void OnShoot(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit) {}

    public void Shoot(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit)
    {
        onShootDelegate(inventory, weapon, damage, hit);
    }

}
