using Stims;
using UnityEngine;
using World.StimListeners;

namespace Enemies
{
    public class EnemyRagdoll : MonoBehaviour
    {

        [SerializeField] private Health health = null;
        
        [SerializeField] private Collider[] colliders = null;
        [SerializeField] private Rigidbody[] rigidbodies = null;

        [SerializeField] private bool ragdollInstantlyOnDeath = true;

        private void EnableRagdoll()
        {
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
            }

            transform.parent = null;
            Debug.Log("Ragdollin");
        }

        private void Start()
        {
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true;
            }
        }

        private void OnEnable() => health.OnDeath.AddListener(OnDeath);
        private void OnDisable() => health.OnDeath.RemoveListener(OnDeath);
        
        private void OnDeath(Stim stim)
        {
            if (ragdollInstantlyOnDeath) EnableRagdoll();
            switch (stim)
            {
                case HitscanDamageStim hitscanStim:
                    OnBulletDeath(hitscanStim);
                    break;
                case Stim.Katana katanaStim:
                    OnKatanaDeath(katanaStim);
                    break;
                case SourcedPointForceStim forceStim:
                    OnBluntForceDeath(forceStim);
                    break;
            }
        }

        private void OnBluntForceDeath(SourcedPointForceStim stim)
        {
            Vector3 baseForce = stim.Force();
            Rigidbody closest = rigidbodies[0];
            float dist = 1f;
            foreach (Rigidbody rb in rigidbodies)
            {
                float newDist = rb.ClosestPointOnBounds(stim.Point()).sqrMagnitude;
                if (newDist < dist)
                {
                    closest = rb;
                    dist = newDist;
                }
            }

            closest.AddForceAtPosition(baseForce, stim.Point());
        }

        private void OnBulletDeath(HitscanDamageStim stim)
        {
            Vector3 baseForce = stim.Force();
            Rigidbody closestRb = rigidbodies[0];
            Vector3 closestPoint = transform.position;
            float closestDistance = float.MaxValue;
            foreach(Rigidbody rb in rigidbodies)
            {
                Vector3 point = rb.ClosestPointOnBounds(stim.Point());
                float dist = Vector3.SqrMagnitude(point - stim.Point());
                if (dist < closestDistance)
                {
                    closestRb = rb;
                    closestPoint = point;
                    closestDistance = dist;
                }
            }
            closestRb.AddForceAtPosition(baseForce, closestPoint, ForceMode.Impulse);
        }

        private void OnKatanaDeath(Stim.Katana stim)
        {
            // todo: implement this baby
            /* Vector3 baseForce = stim.damager.transform.forward * -katanaDeathForce;
            Rigidbody closestRB = rigidbodies[0];
            float closestDistance = float.MaxValue;
            foreach(Rigidbody rb in rigidbodies)
            {
                float newDist = Vector3.SqrMagnitude(rb.position - stim.damager.transform.position);
                if (newDist < closestDistance)
                {
                    closestDistance = newDist;
                    closestRB = rb;
                }
            }
            closestRB.AddForce(baseForce, ForceMode.Impulse); */
        }
    }
}
