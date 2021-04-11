using ScriptableObjects;
using ScriptableObjects.World;
using UnityEngine;
using World;

namespace Weapons.Guns
{
    public static class Hitscan
    {
        public static void FireShot
        (
            float baseDamage, float kineticPower, FalloffEffect falloffEffect, LayerMask layerMask,
            Transform shotTransform = null, float maxRange = Mathf.Infinity, SurfaceMaterial defaultSurfaceMaterial = null,
            GameObject hitPrefab = null, bool forceHitPrefab = false
        ) 
        {

            Ray shotRay = new Ray(shotTransform.position, shotTransform.rotation * Vector3.forward);
            if (Physics.Raycast(shotRay, out RaycastHit hit, maxRange, layerMask))
            {
                // we hit something???
                Transform hitObject = hit.transform;
                // check for rigidbody on hit thing
                Rigidbody hitRB = hitObject.GetComponent<Rigidbody>();
                if (hitRB)
                {
                    hitRB.AddForceAtPosition(shotRay.direction * falloffEffect.Evaluate(kineticPower, hit.distance), hit.point, ForceMode.Impulse);
                }
                
                bool spawnedFX = false;
                if (hitPrefab)
                {
                    SpawnHitFX(hitPrefab, hit);
                    spawnedFX = true;
                }
                // check for damageable on hit thing
                Damageable damageable = hitObject.GetComponent<Damageable>();
                if (damageable)
                {
                    damageable.Damage(new Damage.PlayerBulletDamage(falloffEffect.Evaluate(baseDamage, hit.distance), shotRay.direction, hit));
                    // if we aren't already overriding the fx prefab
                    if (damageable.fxPrefab && !spawnedFX)
                    {
                        SpawnHitFX(damageable.fxPrefab, hit);
                        spawnedFX = true;
                    }
                }
                
                // grab surface material
                SurfaceMaterial material = defaultSurfaceMaterial;
                var worldProperties = hitObject.GetComponent<WorldProperties>();
                if (worldProperties) material = worldProperties.SurfaceMaterial;
                if (material)
                {
                    // do audio
                    var audioEvent = material.GetAudioEvent(HitType.Bullet);
                    if (audioEvent)
                    {
                        AudioEvent.InstantiateEvent(audioEvent, hit.point);
                    }

                    // if we haven't already spawned fx, use world material
                    if (!spawnedFX)
                    {
                        var materialPrefab = material.GetPrefab(HitType.Bullet);
                        if (materialPrefab) SpawnHitFX(materialPrefab, hit);
                    }
                }
            }
        }

        private static void SpawnHitFX(GameObject fxObject, RaycastHit hit)
        {
            Object.Instantiate(fxObject, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
        }
        
    }
}