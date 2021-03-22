using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "AmmoData", menuName = "ScriptableObjects/Weapons/AmmoData", order = 0)]
    public class AmmoData : ScriptableObject
    {
        [SerializeField] private string _name = "Ammo";
        [SerializeField] private int _weight = 15;
        // todo: ammo icon and whatnot
        
        public string Name => _name;
        public int Weight => _weight;
    }
}