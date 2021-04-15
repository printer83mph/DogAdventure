using ScriptableObjects.Audio.Events;
using ScriptableObjects.World;
using Stims.Receivers;
using UnityEngine;
using World;

namespace Stims.Effectors
{
    public class HitscanEffector : MonoBehaviour
    {

        [SerializeField] private HitType hitType = HitType.Bullet;
        [SerializeField] private DamageType damageType = DamageType.Bullet;
        [SerializeField] private SurfaceMaterial defaultSurfaceMaterial = null;
        [SerializeField] private LayerMaskConfig layerMaskConfig = null;
        
        public void Shoot
        (
            float baseDamage, float baseForce, FalloffEffect effect, StimSource source,
            Transform shotTransform = null, float maxRange = Mathf.Infinity,
            GameObject overrideHitPrefab = null, AudioEvent overrideHitAudioEvent = null
        )
        {
            
            // default values
            if (!shotTransform) shotTransform = transform;
            
            Ray shotRay = new Ray(shotTransform.position, shotTransform.rotation * Vector3.forward);
            if (Physics.Raycast(shotRay, out RaycastHit hit, maxRange, layerMaskConfig.Mask))
            {
                // we hit something???
                float force = effect.Evaluate(baseForce, hit.distance);
                // check for rigidbody on hit thing
                Rigidbody hitRB = hit.rigidbody;
                if (hitRB)
                {
                    hitRB.AddForceAtPosition(shotRay.direction * force, hit.point, ForceMode.Impulse);
                }
                
                // check for damageable on hit thing
                StimReceiver stimReceiver = hit.collider.GetComponent<StimReceiver>();
                if (stimReceiver)
                {
                    stimReceiver.Stim(new HitscanDamageStim(effect.Evaluate(baseDamage, hit.distance),
                        damageType, shotRay, hit, force, source));
                }

                SurfaceMaterial material = null;
                WorldProperties properties = hit.collider.GetComponent<WorldProperties>();
                if (properties) material = properties.SurfaceMaterial;
                
                if (overrideHitPrefab)
                {
                    // spawn our own prefab if overriding
                    Object.Instantiate(overrideHitPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
                else
                {
                    // grab surface material
                    SurfaceMaterial.InstantiateHitPrefab(material, hitType, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal),
                        fallback: defaultSurfaceMaterial);
                }

                if (overrideHitAudioEvent)
                {
                    AudioEvent.InstantiateEvent(overrideHitAudioEvent, hit.point);
                }
                else
                {
                    SurfaceMaterial.InstantiateAudioEvent(material, hitType, hit.point,
                        fallback: defaultSurfaceMaterial);
                }
            }
        }
    }
}