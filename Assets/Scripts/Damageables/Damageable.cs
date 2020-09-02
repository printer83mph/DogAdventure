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

public class Damageable : MonoBehaviour
{

    public delegate void OnDamageDelegate(float damage);
    public OnDamageDelegate onDamage = delegate { };

    public delegate void OnShotDelegate(PlayerShotInfo info);
    public OnShotDelegate onShot = delegate { };

    public delegate void OnMeleeDelegate(float damage);
    public OnMeleeDelegate onMelee = delegate { };

    public bool canShoot = true;
    public bool canMelee = true;
    
    public GameObject fxPrefab;

    public void Shoot(PlayerShotInfo info)
    {
        if (!canShoot) return;
        onDamage(info.damage);
        onShot(info);
    }

    public void Melee(float damage)
    {
        if (!canMelee) return;
        onDamage(damage);
        onMelee(damage);
    }

}
