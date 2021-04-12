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
        [SerializeField] private Transform weaponParent = null;
        
        [SerializeField] private WeaponInventoryState[] weapons = null;
        private int _currentWeapon;
        
        // todo: implement ammo data

        [SerializeField] private NewPlayerController controller = null;
        [SerializeField] private PlayerInput input = null;
        [SerializeField] private FistsController fists = null;

        private void OnEnable()
        {
            // assign input
            input.actions["Melee"].performed += Swing;
        }

        private void OnDisable()
        {
            input.actions["Melee"].performed -= Swing;
        }

        private void SwitchToWeapon(int index)
        {
            ClearWeaponObjects();
            
            _currentWeapon = index;
            
            AddWeaponObject(weapons[index].WeaponData);
        }

        private void AddWeaponObject(WeaponData weaponData)
        {
            GameObject newWep = Instantiate(weapons[_currentWeapon].WeaponData.Prefab, weaponParent);
            newWep.GetComponent<Weapon>().Initialize(this, controller, weapons[_currentWeapon], input);
        }

        private void ClearWeaponObjects()
        {
            foreach (Transform child in weaponParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void SetWeaponParentActive(bool active)
        {
            if (weaponParent.gameObject.activeSelf == active) return;
            weaponParent.gameObject.SetActive(active);
        }

        private void Start()
        {

            if (weapons.Length > 0)
            {
                SwitchToWeapon(0);
            }
            else
            {
                BareFists();
            }
        }

        // for when we want just fists and nothing else!
        private void BareFists()
        {
            _currentWeapon = -1;

            fists.Enable();
            fists.Reset();
        }

        private void Swing(InputAction.CallbackContext ctx)
        {
            ClearWeaponObjects();
            if (!fists.Active) fists.Enable();
            fists.RequestSwing();
            StopAllCoroutines();
            StartCoroutine(SwingCoroutine());
        }

        private IEnumerator SwingCoroutine()
        {
            // wait until fists not swinging
            while (true)
            {
                yield return new WaitUntil(() => !fists.Swinging);
                yield return new WaitForSeconds(.2f);
                if (!fists.Swinging) break;
            }

            fists.Disable();
            AddWeaponObject(weapons[_currentWeapon].WeaponData);

        }
    }
}