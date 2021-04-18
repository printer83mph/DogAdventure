using ScriptableObjects.Weapons;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyWeapon : MonoBehaviour
    {

        [SerializeField] private float equipTime = .8f;
        public WeaponData Data { get; private set; }
        public WeaponState State { get; private set; }
        
        [HideInInspector]
        public bool attackMode;
        [HideInInspector]
        public bool shouldMove;

        private float _readyTime;
        public bool Equipping => _readyTime < Time.time;

        public void Initialize(WeaponData data, WeaponState state)
        {
            Data = data;
            State = state;
        }

        private void Start()
        {
            _readyTime = Time.time + equipTime;
        }
    }
}