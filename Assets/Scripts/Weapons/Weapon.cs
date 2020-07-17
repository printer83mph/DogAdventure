using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    
    public float switchTime = .3f;

    public PlayerController playerController;
    public PlayerInventory playerInventory;
    public Camera cam;

    public WeaponFloatData floats;

    public void Equip(PlayerController controller, PlayerInventory inventory, Camera plyCam, WeaponFloatData inFloats)
    {
        playerController = controller;
        playerInventory = inventory;
        cam = plyCam;
        
        playerController.AddToViewmodel(transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        floats = inFloats;
    }

    // has it been long enough since a switch?
    public bool CanFire()
    {
        return Time.time - playerInventory.lastSwitch > switchTime;
    }

    public void SetFloat(String key, float val)
    {
        floats[key] = val;
    }
}
