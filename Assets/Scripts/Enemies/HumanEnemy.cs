using System;
using ScriptableObjects.Audio;
using ScriptableObjects.Enemies;
using Stims;
using UnityEngine;
using Weapons.Enemy;
using World;
using World.StimListeners;

namespace Enemies
{
    public class HumanEnemy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health health;
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private EnemyVision vision;
        [SerializeField] private EnemyWeaponManager weaponManager;
        [SerializeField] private EnemyRagdoll ragdoll;
        
        [SerializeField] private Animator animator;
        [SerializeField] private Animator stateAnimator;

        public Health Health => health;
        public EnemyMovement Movement => movement;
        public EnemyVision Vision => vision;
        public EnemyWeaponManager WeaponManager => weaponManager;
        public EnemyRagdoll Ragdoll => ragdoll;
        
        public Animator Animator => animator;
        public Animator StateAnimator => stateAnimator;

        public Transform Head => Vision.transform;
        public Transform Feet => Movement.FeetTransform;

        [Header("Specific Enemy Config")]
        [SerializeField] private HumanEnemyBehaviourConfig config;
        public HumanEnemyBehaviourConfig Config => config;

        [HideInInspector] public Vector3 lastKnownPosition;
        [HideInInspector] public float stunTimeLeft;

        private void OnDeath(Stim stim)
        {
            animator.enabled = false;
        }

        private void Awake()
        {
            health.OnDeath.AddListener(OnDeath);
        }
        
        private void OnEnable()
        {
            health.enabled = true;
            // animator.enabled = true;
            stateAnimator.enabled = true;
        }

        private void OnDisable()
        {
            health.enabled = false;
            // animator.enabled = false;
            stateAnimator.enabled = false;
        }
    }
}