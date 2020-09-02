using UnityEngine;

public class Weapon : MonoBehaviour
{

    public delegate void EquipEvent(PlayerController controller, PlayerInventory inventory, WeaponSlot slot);
    public EquipEvent onEquip = delegate { };
    
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

        _slot = weaponSlot;
        // if no data provided then update with defaults
        if (weaponSlot.Data.Length == 0) weaponSlot.Data = defaultFloatData;
        
        // run equip delegate
        onEquip(controller, inventory, weaponSlot);
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
