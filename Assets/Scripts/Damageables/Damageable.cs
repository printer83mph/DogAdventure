using UnityEngine;

public class Damageable : MonoBehaviour
{

    public delegate void OnDamageDelegate(Damage damage);
    public OnDamageDelegate onDamage = delegate { };

    public GameObject fxPrefab;

    public void Damage(Damage dmg)
    {
        onDamage(dmg);
    }

}
