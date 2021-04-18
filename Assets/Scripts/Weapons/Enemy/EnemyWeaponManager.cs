using Enemies;
using ScriptableObjects.Weapons;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyWeaponManager : MonoBehaviour
    {
        [SerializeField] private HumanEnemyBehaviour behaviour;
        [SerializeField] private SkinnedMeshRenderer originalRenderer;
        public GameObject WeaponObject { get; private set; }
        public EnemyWeapon WeaponComponent { get; private set; }

        public void SetWeaponPrefab(GameObject prefab, WeaponData data, WeaponState state)
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

            if (!WeaponComponent) return;
            // pass our data and state down to the weapon
            Debug.Log("running initialization thing");
            WeaponComponent.Initialize(data, state, behaviour);
        }

        public void ClearWeaponPrefab()
        {
            Destroy(WeaponObject);
        }
        
    }
}