using System;
using ScriptableObjects.Audio;
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
        [SerializeField] private AudioChannel audioChannel;
        
        [SerializeField] private Animator animator;
        [SerializeField] private Animator stateAnimator;

        public Health Health => health;
        public EnemyMovement Movement => movement;
        public EnemyVision Vision => vision;
        public EnemyWeaponManager WeaponManager => weaponManager;
        public EnemyRagdoll Ragdoll => ragdoll;
        public AudioChannel AudioChannel => audioChannel;
        
        public Animator Animator => animator;
        public Animator StateAnimator => stateAnimator;

        [Header("Specific Enemy Config")]
        [SerializeField] private float suspicionGainSpeed = 50f;
        [SerializeField] private float suspicionDispelSpeed = 1f;
        public float SuspicionGainSpeed => suspicionGainSpeed;
        public float SuspicionDispelSpeed => suspicionDispelSpeed;

        [HideInInspector] public Vector3 lastKnownPosition;

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