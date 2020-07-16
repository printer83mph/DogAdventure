using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Useable))]
public class WeaponPickup : MonoBehaviour
{

    [SerializeField]
    private bool _autoPickUp;

    public WeaponSlot weapon;

    private Useable _useable;
    
    // Start is called before the first frame update
    void Start()
    {
        _useable = GetComponent<Useable>();
        if (_useable)
        {
            _useable.onUseDelegate += PickUp;
        }
    }

    void PickUp(PlayerInventory inventory)
    {
        inventory.AddWeapon(weapon);
        Destroy(gameObject);
    }
}
