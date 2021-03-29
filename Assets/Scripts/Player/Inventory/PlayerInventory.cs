﻿using Player.Controlling;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private Transform weaponParent;
        
        [SerializeField] private WeaponInventoryState[] weapons;
        private int _currentWeapon;
        
        // todo: implement ammo data

        [SerializeField] private NewPlayerController controller;
        [SerializeField] private PlayerInput input;

        private void SwitchToWeapon(int index)
        {
            foreach (Transform child in weaponParent)
            {
                Destroy(child);
            }
            
            _currentWeapon = index;
            GameObject newWep = Instantiate(weapons[index].WeaponData.Prefab, weaponParent);

            newWep.GetComponent<Weapon>().Initialize(this, controller, weapons[index], input);
        }

        private void Start()
        {
            SwitchToWeapon(0);
        }
    }
}