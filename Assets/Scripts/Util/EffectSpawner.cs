using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{

    public Transform muzzle;

    void SpawnMuzzleFlash(GameObject flash)
    {
        Transform spawnedFlash = Instantiate(flash).transform;
        spawnedFlash.position = muzzle.position;
        spawnedFlash.rotation = muzzle.rotation;
        spawnedFlash.parent = muzzle;
    }
}