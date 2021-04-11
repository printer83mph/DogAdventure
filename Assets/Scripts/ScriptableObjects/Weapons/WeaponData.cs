using UnityEngine;

namespace ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapons/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string displayName = null;
        // whatever other weapon data stuff we need
        [SerializeField] private GameObject prefab = null;
        public GameObject Prefab => prefab;
        
        [SerializeField] private int[] defaultInts = null;
        public int[] DefaultInts => defaultInts;
    }
}