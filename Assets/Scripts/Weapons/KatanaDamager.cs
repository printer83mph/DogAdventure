using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.World;
using Stims;
using UnityEngine;
using World;

public class KatanaDamager : MonoBehaviour
{

    [SerializeField] private float damage = 5f;
    [SerializeField] private float hitForce = 2000f;
    [SerializeField] private SurfaceMaterial defaultMaterial = null;
    
    private void OnTriggerEnter(Collider other) {

        Vector3 closestPointHit = other.ClosestPointOnBounds(transform.position);
        var hitRotation = Quaternion.Euler(0, 180, 0) * transform.rotation;
        
        StimReceiver stimReceiver = other.GetComponent<StimReceiver>();
        if (stimReceiver) {
            stimReceiver.Stim(new Stim.Katana(damage, transform.forward * -hitForce, closestPointHit, this));
        }
        
        WorldProperties properties = other.GetComponent<WorldProperties>();
        if (properties)
        {
            var material = properties.SurfaceMaterial;
            material.InstantiateHitPrefab(HitType.Katana, defaultMaterial, closestPointHit, hitRotation);
            material.InstantiateAudioEvent(HitType.Katana, defaultMaterial, closestPointHit);
        }
        
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb) {
            rb.AddForce(transform.forward * -hitForce);
        }
    }

}
