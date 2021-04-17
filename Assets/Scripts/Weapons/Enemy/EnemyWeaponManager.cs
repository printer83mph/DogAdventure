using ScriptableObjects.Weapons;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyWeaponManager : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer originalRenderer;
        public GameObject CurrentWeaponPrefab { get; private set; }

        public void SetWeaponPrefab(GameObject prefab, WeaponData data = null, WeaponState state = null)
        {
            ClearWeaponPrefab();
            CurrentWeaponPrefab = Instantiate(prefab, transform);
            
            // set up rendering correctly
            var renderers = CurrentWeaponPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.bones = originalRenderer.bones;
            }

            // pass our data and state down to the weapon
            if (!(data) && state == null) return;
            var enemyWeapon = CurrentWeaponPrefab.GetComponentInChildren<EnemyWeapon>();
            enemyWeapon.Initialize(data, state);
        }

        public void ClearWeaponPrefab()
        {
            Destroy(CurrentWeaponPrefab);
        }
    }
}