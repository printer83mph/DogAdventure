using System;
using UnityEngine;

[Serializable]
public class WeaponChance
{
    public GameObject weaponPrefab;
    public float quality = 3f;

}

[CreateAssetMenu(fileName="Crate Config", menuName="ScriptableObjects/CrateConfig", order=1)]
public class CrateConfig : ScriptableObject
{

    [Header("Probabilities")]
    public float forceHealthThreshold = 30;
    public float healthMultiplier = 1;
    public float ammoMultiplier = 1;
    public float weaponProbability = .5f;
    [SerializeField]
    public WeaponChance[] weaponChances;

    [Header("Prefabs")]
    public GameObject healthPrefab;
    public GameObject ammoPrefab;

}
