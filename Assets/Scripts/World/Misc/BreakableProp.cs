using Stims;
using UnityEngine;
using World.StimListeners;

namespace World.Misc
{
    public class BreakableProp : MonoBehaviour
    {
        [SerializeField] private Rigidbody propRigidbody = null;
        [SerializeField] private Health health = null;

        [SerializeField] private GameObject breakPrefab;
        [SerializeField] private bool applyStimForce = true;
        [SerializeField] private float forceFromOrigin = 0f;

        // todo: create class called Health (required by this script) that takes a bunch of stim receivers and damage scale per damage type
        // that class can also enable or disable collision stims dealing damage

        private void Start()
        {
            health.OnDeath.AddListener(OnBreak);
        }
        
        private void OnBreak(Stim stim)
        {
            
            if (breakPrefab) {
                GameObject fx = Instantiate(breakPrefab, transform);
                fx.transform.parent = null;
                if (propRigidbody || applyStimForce || forceFromOrigin > 0) TransferVelocity(fx, stim);
            }
            Destroy(gameObject);
        }

        private void TransferVelocity(GameObject obj, Stim stim)
        {

            // check for point force stim
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
            
            // get child rigidbodies
            Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();

            // if nothing GET OUTTA HERE
            if (rbs.Length == 0) return;

            Rigidbody closestRb = null;
            Vector3 closestPoint = default;
            float closestSqrDistance = Mathf.Infinity;
            
            foreach (var rigidbody in rbs)
            {
                // if we inherit velocity
                if (propRigidbody)
                {
                    rigidbody.velocity += propRigidbody.GetPointVelocity(rigidbody.transform.position);
                    rigidbody.angularVelocity += rigidbody.angularVelocity;
                }

                // if we need to add outward force
                if (forceFromOrigin > 0)
                {
                    rigidbody.AddForce(
                        Vector3.Normalize(rigidbody.worldCenterOfMass - transform.position) *
                        forceFromOrigin, ForceMode.Impulse);
                }
                
                if (stimHasPointForce)
                {
                    // do closest point logic if we have point stim force
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
                    // if it's not point force then just add our impulse stim force
                    rigidbody.AddForce(stimForce, ForceMode.Impulse);
                }
            }
            
            // on our actual closest rigidbody we add da force at da position
            if (stimHasPointForce && closestRb)
            {
                closestRb.AddForceAtPosition(stimForce, closestPoint, ForceMode.Impulse);
            }
        }

    }
}
