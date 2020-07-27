using UnityEngine;

public class PlayerShotInfo {

    public PlayerShotInfo(PlayerInventory inventory, Weapon weapon, float damage, RaycastHit hit, Vector3 direction) {
        this.inventory = inventory;
        this.weapon = weapon;
        this.damage = damage;
        this.hit = hit;
        this.direction = direction;
    }

    public PlayerInventory inventory;
    public Weapon weapon;
    public float damage;
    public RaycastHit hit;
    public Vector3 direction;
}

public class Shootable : MonoBehaviour
{

    public delegate void OnShootDelegate(PlayerShotInfo info);

    public OnShootDelegate onShootDelegate;
    
    public GameObject fxPrefab;

    void Start()
    {
        onShootDelegate += OnShoot;
    }
    
    void OnShoot(PlayerShotInfo info) {}

    public void Shoot(PlayerShotInfo info)
    {
        onShootDelegate(info);
    }

}
