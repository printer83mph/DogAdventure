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

[Serializable]
public class Ammo
{
    public string type;
    public int count;
} // TODO: this

[RequireComponent(typeof(PlayerController))]
public class PlayerInventory : MonoBehaviour
{

    [Header("Weapons")]
    // inspector stuff
    [SerializeField]
    public List<WeaponSlot> weapons;
    public List<Ammo> ammo;
    public bool holstered = true;
    public float scrollScale = 10f;

    [Header("Interaction")]
    public float interactDistance = 2f;
    public float maxUseAngle = 30f;
    public LayerMask interactMask = ~0;
    
    [Header("Katana")]
    public bool hasKatana;
    public GameObject katanaPrefab;
    public float katanaSheathTime = .8f;
    public float katanaSwingDelay = .25f;

    // auto-assigned
    private PlayerController _playerController;
    private ViewmodelBob _viewmodelBob;
    private Camera _camera;
    private PlayerHealth _health;
    private PlayerInput _input;

    // scrip references
    public float LastSwitch => _lastSwitch;
    
    // math things
    private int _currentWeaponIndex;
    private float _scrollBuildup;
    private Weapon _currentWeapon;
    private float _lastSwitch;
    private GameObject _katanaObject;
    private float _lastSwing;
    private bool _swinging;
    
    private Useable _thingToUse;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _health = GetComponent<PlayerHealth>();
        _input = GetComponent<PlayerInput>();

        _camera = _playerController.cam;
        _viewmodelBob = _playerController.viewmodelBob;
        
        _health.onDeathDelegate += OnDeath;

        _input.actions["Fire1"].performed += ctx => OnFire();
        _input.actions["WeaponSwitch"].performed += ctx => OnWeaponSwitch(ctx.ReadValue<float>());
        _input.actions["WeaponSlot"].performed += ctx => OnSlotSelect(ctx.ReadValue<float>());
        _input.actions["Use"].performed += _ => OnUse();
        _input.actions["Melee"].performed += _ => OnMelee();
    }

    private void Start()
    {
        // initialize each lists if not already
        if (weapons == null) weapons = new List<WeaponSlot>();
        if (ammo == null) ammo = new List<Ammo>();

        _lastSwitch = Time.time;

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

    private void OnMelee() {
        if (!hasKatana) return;
        // if we can swing again
        if (Time.time - _lastSwing > katanaSwingDelay) {
            if (_swinging) {
                _katanaObject.GetComponent<Animator>().SetTrigger("swing");
            } else {
                if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);
                _katanaObject = Instantiate(katanaPrefab);
                AddToViewmodel(_katanaObject.transform);
            }
            _lastSwing = Time.time;
            _swinging = true;
            holstered = true;
        }
    }

    void Update()
    {

        if (_swinging)
        {
            if (Time.time - _lastSwing > katanaSheathTime)
            {
                Destroy(_katanaObject);
                _swinging = false;
                SwitchToWeapon(_currentWeaponIndex);
            }
        }

        // highlight useable element
        CheckUseables();

    }

    public void AddToViewmodel(Transform newGuy)
    {
        newGuy.parent = _viewmodelBob.transform;
        newGuy.localPosition = Vector3.zero;
        newGuy.localRotation = Quaternion.identity;
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

    // for manual weapon switching
    void SwitchToWeapon(int weaponIndex)
    {
        
        // don't do anything if swinging
        if (_swinging) return;

        // don't do anything if we're switching to the same weapon
        if (weaponIndex == _currentWeaponIndex && !holstered) return;
        if (weaponIndex >= weapons.Count) return;
        
        InstantiateWeaponSlot(weaponIndex);

    }

    private void InstantiateWeaponSlot(int weaponIndex) {

        _currentWeaponIndex = weaponIndex;

        // destroy current weapon gameobject and create new one
        if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);
        _currentWeapon = Instantiate(weapons[weaponIndex].Weapon).GetComponent<Weapon>();
        _currentWeapon.Equip(_playerController, this, weapons[weaponIndex]);
        _lastSwitch = Time.time;

        holstered = false;
    }

    public void AddWeapon(WeaponSlot weaponSlot)
    {
        // TODO: optimize this
        if (weapons.Count == 1 && weapons[0].Weapon.tag == "KatanaWeapon")
        {
            weapons.Clear();
            Debug.Log("actual gun!!");
        }
        else if (weaponSlot.Weapon.tag == "KatanaWeapon")
        {
            hasKatana = true;
            if (weapons.Count > 0) return;
        }
        weapons.Add(weaponSlot);
        InstantiateWeaponSlot(weapons.Count - 1);
    }

    private void OnDeath()
    {
        // delete all children
        foreach (Transform trans in _camera.transform) {
            Destroy(trans.gameObject);
        }
        this.enabled = false;
    }
}
