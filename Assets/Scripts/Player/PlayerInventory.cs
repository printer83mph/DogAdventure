using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInventory : MonoBehaviour
{

    public List<GameObject> weapons;
    public bool holstered = true;

    private PlayerController _playerController;
    
    private int _currentWeaponIndex;
    private Weapon _currentWeapon;
    [HideInInspector]
    public float lastSwitch;
    
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
        _currentWeapon = Instantiate(weapons[weaponIndex]).GetComponent<Weapon>();
        _currentWeapon.Equip(_playerController, this);
        lastSwitch = Time.time;

        holstered = false;
    }
}
