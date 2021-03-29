using System;
using UnityEngine;

public class OldWeapon : MonoBehaviour
{

    public delegate void EquipEvent(PlayerController controller, OldPlayerInventory inventory, WeaponSlot slot);
    public EquipEvent onEquip = delegate { };

    public String displayName;
    public float switchTime = .3f;

    public PlayerController playerController;
    public OldPlayerInventory oldPlayerInventory;
    public Camera cam;

    private WeaponSlot _slot;

    [SerializeField]
    public float[] defaultFloatData;

    public void Equip(PlayerController controller, OldPlayerInventory inventory, WeaponSlot weaponSlot)
    {
        playerController = controller;
        oldPlayerInventory = inventory;
        cam = controller.cam;
        
        oldPlayerInventory.AddToViewmodel(transform);

        _slot = weaponSlot;
        // if no data provided then update with defaults
        if (weaponSlot.Data.Length == 0) weaponSlot.Data = defaultFloatData;
        
        // run equip delegate
        onEquip(controller, inventory, weaponSlot);
    }

    // has it been long enough since a switch?
    public bool CanFire()
    {
        return Time.time - oldPlayerInventory.LastSwitch > switchTime;
    }

    public void SetFloat(int index, float val)
    {
        _slot.Data[index] = val;
    }

    public float GetFloat(int index) => _slot.Data[index];
    
}
