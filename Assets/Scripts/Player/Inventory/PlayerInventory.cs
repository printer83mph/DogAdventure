using System;
using System.Collections;
using Player.Controlling;
using ScriptableObjects.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {

        public PlayerInventory Main {get; private set; }
        
        [Serializable]
        private class WeaponInventorySlot
        {
            public WeaponData data;
            public WeaponState state;
        }

        [SerializeField] private Transform weaponParent = null;
        
        [SerializeField] private WeaponInventorySlot[] weapons = null;

        private bool _usingFists;
        private int _currentWeapon;
        
        // todo: implement ammo data

        [SerializeField] private PlayerController controller = null;
        [SerializeField] private PlayerInput input = null;
        [SerializeField] private FistsController fists = null;

        private WeaponInventorySlot CurrentWeapon()
        {
            try
            {
                return weapons[_currentWeapon];
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
            }
        }

        private void Awake()
        {
            Main = this;
        }
        
        private void OnEnable()
        {
            // assign input
            input.actions["Melee"].performed += Swing;
            input.actions["Holster"].performed += Holster;
        }

        private void OnDisable()
        {
            ClearWeaponObjects();
            fists.Disable();
            input.actions["Melee"].performed -= Swing;
            input.actions["Holster"].performed -= Holster;
        }

        private void TrySwitchToWeapon(int index)
        {
            Debug.Log("trying index " + index);
            if (index >= weapons.Length)
            {
                // if unsuccessful, switch to fists
                SwitchToUsingFists();
                return;
            }
            // switch if successful
            fists.Disable();
            SwitchToWeapon(index);

        }

        private void SwitchToWeapon(int index)
        {
            ClearWeaponObjects();
            
            _currentWeapon = index;
            
            AddWeaponObject(weapons[index].data);
        }

        private void AddWeaponObject(WeaponData weaponData)
        {
            var weaponSlot = CurrentWeapon();
            GameObject newWep = Instantiate(weaponSlot.data.PlayerPrefab, weaponParent);
            newWep.GetComponent<PlayerInventoryWeapon>().Initialize(weaponSlot.data, weaponSlot.state);
        }

        private void ClearWeaponObjects()
        {
            foreach (Transform child in weaponParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void Start()
        {

            if (weapons.Length > 0)
            {
                SwitchToWeapon(0);
            }
            else
            {
                SwitchToUsingFists();
            }
        }

        // for when we want just fists and nothing else!
        private void SwitchToUsingFists()
        {
            _usingFists = true;

            ClearWeaponObjects();
            
            if (fists.Active) return;
            
            fists.Enable();
            fists.Reset();
        }

        private void Holster(InputAction.CallbackContext ctx)
        {
            // for later: make sure to check if current weapon is switch-offable
            SwitchToUsingFists();
        }

        private void Swing(InputAction.CallbackContext ctx)
        {
            // for later: make sure to check if current weapon is switch-offable
            ClearWeaponObjects();
            if (!fists.Active) fists.Enable();
            fists.RequestSwing();
            StopAllCoroutines();
            StartCoroutine(SwingCoroutine());
        }

        private IEnumerator SwingCoroutine()
        {
            // wait until fists not swinging
            yield return new WaitUntil(() => !fists.Swinging);
            yield return new WaitForSeconds(.2f);
            
            // as long as we haven't been cancelled we're done with fists

            // if we're not using fists try to switch back to current weapon
            Debug.Log("We made it! put the fists away.");
            if (!_usingFists)
            {
                TrySwitchToWeapon(_currentWeapon);
            }
        }
    }
}