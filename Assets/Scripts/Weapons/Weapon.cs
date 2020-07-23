using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    
    public float switchTime = .3f;

    public PlayerController playerController;
    public PlayerInventory playerInventory;
    public Camera cam;

    private WeaponSlot _slot;

    [SerializeField]
    public float[] defaultFloatData;

    public void Equip(PlayerController controller, PlayerInventory inventory, WeaponSlot weaponSlot)
    {
        playerController = controller;
        playerInventory = inventory;
        cam = controller.cam;
        
        playerInventory.AddToViewmodel(transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _slot = weaponSlot;
        // if no data provided then update with defaults
        if (weaponSlot.Data.Length == 0) weaponSlot.Data = defaultFloatData;
    }

    public void OnEquip() {
        
    }

    // has it been long enough since a switch?
    public bool CanFire()
    {
        return Time.time - playerInventory.lastSwitch > switchTime;
    }

    public void SetFloat(int index, float val)
    {
        _slot.Data[index] = val;
    }

    public float GetFloat(int index) => _slot.Data[index];

}
