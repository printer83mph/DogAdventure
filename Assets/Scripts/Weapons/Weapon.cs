using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float switchTime = .3f;

    public PlayerController playerController;
    public PlayerInventory playerInventory;

    public void Equip(PlayerController controller, PlayerInventory inventory)
    {
        playerController = controller;
        playerInventory = inventory;
        
        playerController.ParentToCamera(transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // has it been long enough since a switch?
    public bool CanFire()
    {
        return Time.time - playerInventory.lastSwitch > switchTime;
    }
}
