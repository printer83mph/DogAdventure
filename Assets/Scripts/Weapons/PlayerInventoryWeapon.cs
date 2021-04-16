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
        public bool Equipping { get; private set; }

        private void Start()
        {
            StartCoroutine(EquipCoroutine());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Initialize(WeaponData data, WeaponState state)
        {
            WeaponData = data;
            State = state;
        }
        
        private IEnumerator EquipCoroutine()
        {
            Equipping = true;
            yield return new WaitForSeconds(equipTime);
            Equipping = false;
        }
    }
}