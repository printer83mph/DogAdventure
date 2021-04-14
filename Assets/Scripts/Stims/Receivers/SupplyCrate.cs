using UnityEngine;

[RequireComponent(typeof(BreakableProp))]
public class SupplyCrate : MonoBehaviour
{

    public CrateConfig crateConfig;
    // private PlayerHealth _health;

    private void Awake()
    {
        // _health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        GetComponent<BreakableProp>().onBreak += OnBreak;
    }

    private void OnBreak()
    {

        

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
        float rand = 0;
        
        foreach (WeaponChance chance in crateConfig.weaponChances)
        {
            rand += 10 - chance.quality;
        }

        rand = UnityEngine.Random.Range(0, rand);

        // TODO: check to see if player already has weapon
        foreach (WeaponChance chance in crateConfig.weaponChances)
        {
            if (rand < 10 - chance.quality)
            {
                Instantiate(chance.weaponPrefab, transform.position, transform.rotation);
                return;
            }
            rand -= 10 - chance.quality;
        }
    }

}
