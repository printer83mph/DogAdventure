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
    public float scrollScale = 10f;
    public LayerMask interactMask = ~0;

    private PlayerController _playerController;
    private Camera _camera;
    private PlayerHealth _health;

    public float maxUseAngle = 30f;
    
    private int _currentWeaponIndex;
    private float _scrollBuildup;
    private Weapon _currentWeapon;
    [HideInInspector]
    public float lastSwitch;
    
    private Useable _thingToUse;

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
        _health = GetComponent<PlayerHealth>();
        _health.onDeathDelegate += OnDeath;
        _camera = _playerController.cam;
        lastSwitch = Time.time;
        if (!holstered)
        {
            SwitchToWeapon(0);
        }
    }

    void Update()
    {

        if (_health.Dead) return;

        int newSlot = KeySwitchWeaponSlot();
        if (newSlot > -1) {
            SwitchToWeapon( newSlot );
        }
        int scrollMeaning = ScrollMeaning();
        if (holstered && weapons.Count != 0)
        {
            if (Input.GetButton("Fire1") || scrollMeaning != 0)
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
        {
            SwitchWeapons( scrollMeaning );
        }

        // highlight useable element
        CheckUseables();
        if (Input.GetButtonDown("Use"))
        {
            if (_thingToUse)
            {
                _thingToUse.Use(this);
            }
        }

    }

    private int ScrollMeaning() {
        float deltaScroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Sign(deltaScroll * _scrollBuildup) < 0) {
            _scrollBuildup = 0;
        }
        _scrollBuildup = Mathf.MoveTowards(_scrollBuildup, 0, Time.deltaTime * 3f);
        _scrollBuildup += deltaScroll * scrollScale;
        if (Mathf.Abs(_scrollBuildup) >= 1) {
            int scrollAmt = (int)Mathf.Sign(_scrollBuildup);
            _scrollBuildup = 0;
            return scrollAmt;
        } else {
            return 0;
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

    // returns whatever weapon we've switched to with the keyboard
    public int KeySwitchWeaponSlot() {
        int outVal = -1;
        for (int i = 0; i < weapons.Count && i < 4; i++) {
            if (Input.GetButton("Weapon Slot " + (i + 1))) {
                outVal = i;
            }
        }
        return outVal;
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

    private void OnDeath() {
        // delete all children
        foreach (Transform trans in _camera.transform) {
            Destroy(trans.gameObject);
        }
    }
}
