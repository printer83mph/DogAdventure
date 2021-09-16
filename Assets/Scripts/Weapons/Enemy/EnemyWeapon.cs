using Enemies;
using ScriptableObjects.Weapons;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyWeapon : MonoBehaviour
    {

        [SerializeField] private float equipTime = .8f;
        public WeaponData Data { get; private set; }
        public WeaponState State { get; private set; }
        public HumanEnemy Enemy { get; private set; }
        
        // to be modified by our behaviour
        [HideInInspector] public bool attackMode;

        private float _readyTime;
        public bool Equipping => _readyTime < Time.time;

        public void Initialize(WeaponData data, WeaponState state, HumanEnemy enemy)
        {
            Data = data;
            State = state;
            Enemy = enemy;
            Debug.Log("Initialized enemy weapon");
        }

        private void Start()
        {
            _readyTime = Time.time + equipTime;
        }
    }
}