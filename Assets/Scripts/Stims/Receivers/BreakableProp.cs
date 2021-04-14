using System;
using UnityEngine;

namespace Stims.Receivers
{
    public class BreakableProp : MonoBehaviour
    {

        public Action onBreak = delegate { };
    
        public GameObject breakPrefab;
    
        [SerializeField] private Health health = null;

        [SerializeField] private bool transferVel = true;
        [SerializeField] private float forceFromOrigin = 0f;

        void Awake()
        {
            health.onDeath += Break;
        }
    
        // todo: create class called Health (required by this script) that takes a bunch of stim receivers and damage scale per damage type
        // that class can also enable or disable collision stims dealing damage

        void Break(IStimDamage stim)
        {
            if (breakPrefab) {
                GameObject fx = Instantiate(breakPrefab, transform);
                fx.transform.parent = null;
                if (transferVel) TransferVelocity(fx, stim);
            }
            Destroy(gameObject);
            onBreak();
        }

        private void TransferVelocity(GameObject obj, IStimDamage stim)
        {
            Rigidbody originalRb = GetComponent<Rigidbody>();
            if (!originalRb) return;

            Vector3 stimForce = default;
            bool stimHasPointForce = false;
            Vector3 stimForcePoint = default;
            if (stim is IStimForce forceStim)
            {
                stimForce = forceStim.Force();
                if (stim is IStimPointForce pointForceStim)
                {
                    stimForcePoint = pointForceStim.Point();
                    stimHasPointForce = true;
                }
            }
            
            Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();

            if (rbs.Length == 0) return;
            
            Rigidbody closestRb = null;
            Vector3 closestPoint = default;
            float closestSqrDistance = Mathf.Infinity;
            foreach (var rigidbody in rbs)
            {
                rigidbody.velocity = originalRb.GetPointVelocity(rigidbody.transform.position);
                rigidbody.angularVelocity = originalRb.angularVelocity;

                if (forceFromOrigin > 0)
                {
                    rigidbody.AddForce(Vector3.Normalize(rigidbody.worldCenterOfMass - originalRb.worldCenterOfMass) * forceFromOrigin);
                }

                if (stimHasPointForce)
                {
                    closestPoint = rigidbody.ClosestPointOnBounds(stimForcePoint);
                    float sqrDistance = Vector3.SqrMagnitude(closestPoint - stimForcePoint);
                    if (sqrDistance < Mathf.Pow(closestSqrDistance, 2))
                    {
                        closestRb = rigidbody;
                        closestSqrDistance = sqrDistance;
                    }
                }
                else
                {
                    rigidbody.AddForce(stimForce, ForceMode.Impulse);
                }
            }
            
            // add point force to closest guy
            if (stimHasPointForce && closestRb)
            {
                closestRb.AddForceAtPosition(stimForce, closestPoint, ForceMode.Impulse);
            }
        }

    }
}
