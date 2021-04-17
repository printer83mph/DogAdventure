using System.Collections;
using ScriptableObjects.Weapons;
using UnityEngine;

namespace Weapons
{
    // to pass down our weapon data/state and see if we're equipping
    public class PlayerInventoryWeapon : MonoBehaviour
    {
        [SerializeField] private float equipTime = .3f;
        public WeaponData WeaponData { get; private set; }
        public WeaponState State { get; set; }

        private float _readyTime;
        public bool Equipping => Time.time < _readyTime;

        private void Start()
        {
            _readyTime = Time.time + equipTime;
        }
        
        public void Initialize(WeaponData data, WeaponState state)
        {
            WeaponData = data;
            State = state;
        }
        
    }
}