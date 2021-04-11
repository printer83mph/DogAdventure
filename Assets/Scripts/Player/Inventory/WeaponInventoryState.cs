using System;
using ScriptableObjects;
using ScriptableObjects.Weapons;
using UnityEngine;

namespace Player.Inventory
{
    [Serializable]
    public class WeaponInventoryState
    {
        [SerializeField] private WeaponData weaponData = null;
        [SerializeField] private int[] ints = null;

        public WeaponData WeaponData => weaponData;

        public void Initialize()
        {
            ints = weaponData.DefaultInts;
        }

        public int GetInt(int index) => ints[index];

        public void SetInt(int index, int newVal)
        {
            ints[index] = newVal;
        }
    }
}