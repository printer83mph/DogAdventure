using UnityEngine;

[RequireComponent(typeof(Useable))]
public class WeaponPickup : MonoBehaviour
{

    [SerializeField]
    private bool _autoPickUp;

    public WeaponSlot weapon;

    private Useable _useable;

    void Awake() {
        _useable = GetComponent<Useable>();
    }

    void OnEnable() {
        _useable.onUseDelegate += OnUse;
    }

    void OnDisable() {
        _useable.onUseDelegate -= OnUse;
    }

    void OnUse(PlayerInventory inventory)
    {
        inventory.AddWeapon(weapon);
        Destroy(gameObject);
    }
}
