using System;
using System.Collections.Generic;
using UnityEngine;

// todo: this still isn't serializable
[Serializable]
public class WeaponFloatData : Dictionary<String, float> { }

[Serializable]
public class WeaponSlot
{
    public GameObject Weapon;
    [SerializeField]
    public WeaponFloatData Floats;
}

[RequireComponent(typeof(PlayerController))]
public class PlayerInventory : MonoBehaviour
{

    public List<WeaponSlot> weapons;
    public bool holstered = true;

    private PlayerController _playerController;
    
    private int _currentWeaponIndex;
    private Weapon _currentWeapon;
    [HideInInspector]
    public float lastSwitch;

    private void Awake()
    {
        // initialize each weaponslot floats dict if not already
        foreach (WeaponSlot weaponSlot in weapons)
        {
            if (weaponSlot.Floats == null)
            {
                weaponSlot.Floats = new WeaponFloatData();
            }
        }
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        lastSwitch = Time.time;
        if (!holstered)
        {
            SwitchToWeapon(0);
        }
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        int scrollMeaning = scrollInput == 0 ? 0 : Mathf.RoundToInt(Mathf.Sign(scrollInput));
        if (holstered && weapons.Count != 0)
        {
            if (Input.GetMouseButtonDown(0) || scrollMeaning != 0)
            {
                SwitchToWeapon(_currentWeaponIndex);
            }
        }
        else
        {
            SwitchWeapons( scrollMeaning );
        }
    }

    // shift weapon slot
    void SwitchWeapons(int amt)
    {
        if (amt == 0) return;
        if (weapons.Count == 0) return;
        SwitchToWeapon((_currentWeaponIndex + weapons.Count + amt) % (weapons.Count));
    }

    // actually do the switch
    void SwitchToWeapon(int weaponIndex)
    {
        _currentWeaponIndex = weaponIndex;
        
        // destroy current weapon gameobject and create new one
        if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);
        _currentWeapon = Instantiate(weapons[weaponIndex].Weapon).GetComponent<Weapon>();
        _currentWeapon.Equip(_playerController, this, weapons[weaponIndex].Floats);
        lastSwitch = Time.time;

        holstered = false;
    }
}
