using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapons/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string displayName;
        // whatever other weapon data stuff we need
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;
        
        [SerializeField] private int[] defaultInts;
        public int[] DefaultInts => defaultInts;
    }
}