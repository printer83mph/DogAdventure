using UnityEngine;

[RequireComponent(typeof(BreakableProp))]
public class SupplyCrate : MonoBehaviour
{

    public CrateConfig crateConfig;
    private PlayerHealth _health;

    private void Awake()
    {
        _health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        GetComponent<BreakableProp>().onBreak += OnBreak;
    }

    private void OnBreak()
    {

        if (_health.health < crateConfig.forceHealthThreshold) {
            SpawnHealth();
            return;
        }

        float total = 0;

        float healthProb = (_health.maxHealth - _health.health) / _health.maxHealth * crateConfig.healthMultiplier;
        total += healthProb;

        // TODO: probably ask for 
        float ammoProb = 0;
        total += ammoProb;

        float rand = UnityEngine.Random.Range(0f, total);

        if (rand < healthProb)
        {
            SpawnHealth();
            return;
        }
        total -= healthProb;

        if (rand < ammoProb) {
            SpawnAmmo();
            return;
        }

        // otherwise
        SpawnRandomWeapon();

    }

    private void SpawnHealth()
    {
        Instantiate(crateConfig.healthPrefab, transform.position, transform.rotation);
    }

    private void SpawnAmmo()
    {
        Instantiate(crateConfig.ammoPrefab, transform.position, transform.rotation);
    }

    private void SpawnRandomWeapon()
    {
        float total = 0;
        
        foreach (WeaponChance chance in crateConfig.weaponChances)
        {
            total += 1/chance.quality;
        }

        float rand = UnityEngine.Random.Range(0, total);

        // TODO: check to see if player already has weapon
        foreach (WeaponChance chance in crateConfig.weaponChances)
        {
            if (rand < 1/chance.quality)
            {
                Instantiate(chance.weaponPrefab, transform.position, transform.rotation);
                return;
            }
            total -= 1/chance.quality;
        }
    }

}
