using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSlot
{
    public GameObject Weapon;
    public float[] Data;
}

[RequireComponent(typeof(PlayerController))]
public class PlayerInventory : MonoBehaviour
{

    [SerializeField]
    public List<WeaponSlot> weapons;
    public bool holstered = true;
    public float interactDistance = 2f;
    public LayerMask interactMask = ~0;

    private PlayerController _playerController;
    private Camera _camera;

    public float maxUseAngle = 30f;
    
    private int _currentWeaponIndex;
    private Weapon _currentWeapon;
    [HideInInspector]
    public float lastSwitch;
    
    private Useable _thingToUse;
    private bool _using;

    private void Awake()
    {
        // initialize each weaponslot floats dict if not already
        if (weapons == null)
        {
            weapons = new List<WeaponSlot>();
        }
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _camera = Camera.main;
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
        else if (Input.GetAxis("Holster") > 0)
        {
            // holster the weapon
            holstered = true;
            if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);
            _currentWeaponIndex = 0;
        } else if (scrollMeaning != 0)
        // TODO: use numbers to switch weapons
        {
            SwitchWeapons( scrollMeaning );
        }

        // highlight useable element
        CheckUseables();
        if (Input.GetAxis("Use") > 0)
        {
            if (_thingToUse && !_using)
            {
                _thingToUse.Use(this);
                _using = true;
            }
        }
        else
        {
            _using = false;
        }

    }

    void CheckUseables()
    {
        _thingToUse = null;
        // break if list is empty
        if (Useable.useables.Count == 0)
        {
            return;
        }
        // limit use angle? idk
        float closestAngle = maxUseAngle;
        foreach (Useable useable in Useable.useables)
        {
            useable.highlighted = false;
            Vector3 vFromCam = useable.transform.position - _camera.transform.position;
            Quaternion rotFromCamera =
                Quaternion.FromToRotation(vFromCam, _camera.transform.forward);
            float useableAngle = Quaternion.Angle(rotFromCamera, Quaternion.identity);
            if (useableAngle < closestAngle)
            {
                // make sure we have los
                if (Physics.Raycast(_camera.transform.position, vFromCam, out RaycastHit hit, interactDistance, interactMask)) {
                    if (hit.transform == useable.transform) {
                        _thingToUse = useable;
                        closestAngle = useableAngle;
                    }
                }
            }
        }
        
        if (_thingToUse) _thingToUse.highlighted = true;
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
        if (weaponIndex == _currentWeaponIndex && !holstered) return;
        _currentWeaponIndex = weaponIndex;
        
        // destroy current weapon gameobject and create new one
        if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);
        _currentWeapon = Instantiate(weapons[weaponIndex].Weapon).GetComponent<Weapon>();
        _currentWeapon.Equip(_playerController, this, _camera, weapons[weaponIndex]);
        lastSwitch = Time.time;

        holstered = false;
    }

    public void AddWeapon(WeaponSlot weaponSlot)
    {
        weapons.Add(weaponSlot);
        SwitchToWeapon(weapons.Count - 1);
    }
}
