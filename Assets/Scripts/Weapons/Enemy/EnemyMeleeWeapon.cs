using Player.Controlling;
using UnityEngine;

namespace Weapons.Enemy
{
    public class EnemyMeleeWeapon : MonoBehaviour
    {
        [SerializeField] private EnemyWeapon weapon;
        [SerializeField] private float attackRange = 1.3f;
        [SerializeField] private float attackDelay = .3f;
        [SerializeField] private float attackCooldown = .7f;

        private float _nextAttack;
        private float _nextHit;
        private bool _currentlyAttacking;

        private void Start()
        {
            weapon.Enemy.Movement.locked = false;
        }
        
        private void Update()
        {
            if (!weapon.attackMode) return;
            weapon.Enemy.Movement.Target = weapon.Enemy.lastKnownPosition;
            if (_currentlyAttacking)
            {
                weapon.Enemy.Movement.locked = true;
                Debug.Log("WE SHOULD BE LOCKED !!");
                // just keep going if we're still running attack
                if (Time.time < _nextHit) return;
                // check if we're still in radius
                if (Vector3.SqrMagnitude(weapon.Enemy.Vision.transform.position -
                                         PlayerController.Main.Orientation.position) > attackRange)
                {
                    // we missed
                    Debug.Log("We missed...");
                }
                else
                {
                    // we hit
                    Debug.Log("Hit you!!!");
                }

                _currentlyAttacking = false;
            }
            else
            {
                bool withinDistance = Vector3.SqrMagnitude(weapon.Enemy.Vision.transform.position -
                                                           PlayerController.Main.Orientation.position) <
                                      Mathf.Pow(attackRange, 2);
                weapon.Enemy.Movement.locked = withinDistance;
                if (Time.time < _nextAttack) return;
                if (withinDistance)
                {
                    // start attack
                    _nextAttack = Time.time + attackCooldown;
                    _nextHit = Time.time + attackDelay;
                    _currentlyAttacking = true;
                
                    Debug.Log("Attacking");
                    weapon.Enemy.Animator.Play("WeaponAttack");
                }
            }
        }
    }
}