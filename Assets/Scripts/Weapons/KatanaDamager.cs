using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class KatanaDamager : MonoBehaviour
{

    [SerializeField] private GameObject hitFX = null;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float hitForce = 2000f;
    [SerializeField] private AudioEvent audioEvent = null;
    
    private void OnTriggerEnter(Collider other) {

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable) {
            damageable.Damage(new Damage.PlayerKatanaDamage(damage, this));
        }
        bool hasDamageableFX = damageable && damageable.fxPrefab;
        if (hitFX || hasDamageableFX) {
            Transform hitFXTransform = Instantiate(hasDamageableFX ? damageable.fxPrefab : hitFX).transform;
            hitFXTransform.transform.position = other.ClosestPointOnBounds(transform.position);
            hitFXTransform.transform.rotation = Quaternion.Euler(0, 180, 0) * transform.rotation;
        }
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb) {
            rb.AddForce(transform.forward * -hitForce);
        }
        AudioEvent.InstantiateEvent(audioEvent, transform.position, 3f);
    }

}
