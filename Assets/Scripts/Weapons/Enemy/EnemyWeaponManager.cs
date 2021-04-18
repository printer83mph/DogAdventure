using ScriptableObjects.Weapons;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyWeaponManager : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer originalRenderer;
        public GameObject WeaponObject { get; private set; }
        public EnemyWeapon WeaponComponent { get; private set; }

        public void SetWeaponPrefab(GameObject prefab, WeaponData data = null, WeaponState state = null)
        {
            ClearWeaponPrefab();
            WeaponObject = Instantiate(prefab, transform);
            
            // set up rendering correctly
            var renderers = WeaponObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.bones = originalRenderer.bones;
            }

            WeaponComponent = WeaponObject.GetComponentInChildren<EnemyWeapon>();
            
            // pass our data and state down to the weapon
            if (!(data) && state == null) return;
            WeaponComponent.Initialize(data, state);
        }

        public void ClearWeaponPrefab()
        {
            Destroy(WeaponObject);
        }
        
    }
}