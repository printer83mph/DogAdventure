using ScriptableObjects;
using ScriptableObjects.Audio.Events;
using ScriptableObjects.World;
using Stims;
using UnityEngine;
using World;

namespace Weapons.Guns
{
    public class HitscanShooter : MonoBehaviour
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
                Transform hitObject = hit.transform;
                float force = effect.Evaluate(baseForce, hit.distance);
                // check for rigidbody on hit thing
                Rigidbody hitRB = hitObject.GetComponent<Rigidbody>();
                if (hitRB)
                {
                    hitRB.AddForceAtPosition(shotRay.direction * force, hit.point, ForceMode.Impulse);
                }
                
                // check for damageable on hit thing
                StimReceiver stimReceiver = hitObject.GetComponent<StimReceiver>();
                if (stimReceiver)
                {
                    stimReceiver.Stim(new HitscanDamageStim(effect.Evaluate(baseDamage, hit.distance),
                        damageType, shotRay, hit, force, source));
                }

                Debug.Log("Getting surface material component");
                var material = defaultSurfaceMaterial;
                WorldProperties properties = hitObject.GetComponent<WorldProperties>();
                if (properties)
                {
                    Debug.Log("Using hit object's surface properties");
                    if (!material) material = properties.SurfaceMaterial;
                } else Debug.Log("Using default surface properties");
                
                if (overrideHitPrefab)
                {
                    // spawn our own prefab if overriding
                    Object.Instantiate(overrideHitPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
                else
                {
                    // grab surface material
                    material.InstantiateHitPrefab(hitType, position: hit.point,
                        rotation: Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                if (overrideHitAudioEvent)
                {
                    AudioEvent.InstantiateEvent(overrideHitAudioEvent, hit.point);
                }
                else
                {
                    AudioEvent.InstantiateEvent(material.GetAudioEvent(hitType));
                }
            }
        }
    }
}