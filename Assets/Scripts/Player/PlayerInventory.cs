using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class WeaponSlot
{
    public GameObject Weapon;
    public float[] Data;
}

[RequireComponent(typeof(PlayerController))]
public class PlayerInventory : MonoBehaviour
{

    // inspector stuff
    [SerializeField]
    public List<WeaponSlot> weapons;
    public bool holstered = true;
    public float interactDistance = 2f;
    public float scrollScale = 10f;
    public LayerMask interactMask = ~0;
    public float maxUseAngle = 30f;

    // auto-assigned
    private PlayerController _playerController;
    private ViewmodelBob _viewmodelBob;
    private Camera _camera;
    private PlayerHealth _health;
    private PlayerInput _input;
    
    // math things
    private int _currentWeaponIndex;
    private float _scrollBuildup;
    private Weapon _currentWeapon;
    [HideInInspector]
    public float lastSwitch;
    // TODO: add swinging feature
    
    private Useable _thingToUse;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _health = GetComponent<PlayerHealth>();

        _camera = _playerController.cam;
        _viewmodelBob = _playerController.viewmodelBob;

        // initialize each weaponslot floats dict if not already
        if (weapons == null)
        {
            weapons = new List<WeaponSlot>();
        }

        _input = GetComponent<PlayerInput>();
        _input.actions["Fire1"].performed += ctx => OnFire();
        _input.actions["WeaponSwitch"].performed += ctx => OnWeaponSwitch(ctx.ReadValue<float>());
        _input.actions["WeaponSlot"].performed += ctx => OnSlotSelect(ctx.ReadValue<float>());
        _input.actions["Use"].performed += _ => OnUse();
    }

    void OnEnable() {
        _health.onDeathDelegate += OnDeath;

        lastSwitch = Time.time;
        if (!holstered)
        {
            SwitchToWeapon(0);
        }
    }

    private void OnWeaponSwitch(float amt)
    {
        int actuAmt = (int) amt;
        if (weapons.Count == 0 || actuAmt == 0) return;
        if (holstered) {
            SwitchToWeapon(_currentWeaponIndex);
        } else {
            SwitchWeapons( (int)Mathf.Sign(amt) );
        }
    }

    private void OnSlotSelect(float newSlot)
    {
        int actualNewSlot = (int) newSlot;
        if (actualNewSlot == 0) return;
        SwitchToWeapon( (int)newSlot - 1 );
    }

    private void OnFire()
    {
        if (holstered && weapons.Count > 0) {
            SwitchToWeapon(_currentWeaponIndex);
            return;
        }
    }

    private void OnUse()
    {
        if (_thingToUse)
        {
            _thingToUse.Use(this);
        }
    }

    void Update()
    {

        if (_health.Dead) return;

        // highlight useable element
        CheckUseables();

    }

    public void AddToViewmodel(Transform newGuy)
    {
        newGuy.parent = _viewmodelBob.transform;
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
        // don't do anything if we're switching to the same weapon
        if (weaponIndex == _currentWeaponIndex && !holstered) return;
        if (weaponIndex >= weapons.Count) return;
        _currentWeaponIndex = weaponIndex;
        
        // destroy current weapon gameobject and create new one
        if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);
        _currentWeapon = Instantiate(weapons[weaponIndex].Weapon).GetComponent<Weapon>();
        _currentWeapon.Equip(_playerController, this, weapons[weaponIndex]);
        lastSwitch = Time.time;

        holstered = false;
    }

    public void AddWeapon(WeaponSlot weaponSlot)
    {
        weapons.Add(weaponSlot);
        SwitchToWeapon(weapons.Count - 1);
    }

    private void OnDeath()
    {
        // delete all children
        foreach (Transform trans in _camera.transform) {
            Destroy(trans.gameObject);
        }
    }
}
