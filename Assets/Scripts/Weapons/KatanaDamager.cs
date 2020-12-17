﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaDamager : MonoBehaviour
{

    public GameObject hitFX;
    public float damage = 5f;
    public float hitForce = 2000f;
    
    private void OnTriggerEnter(Collider other) {

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable) {
            damageable.Damage(new Damage.PlayerKatanaDamage(damage));
        }
        bool hasDamageableFX = damageable && damageable.fxPrefab;
        if (hitFX || hasDamageableFX) {
            Transform hitFXTransform = Instantiate(hasDamageableFX ? damageable.fxPrefab : hitFX).transform;
            hitFXTransform.transform.position = other.ClosestPointOnBounds(transform.position);
            hitFXTransform.transform.rotation = Quaternion.Euler(0, 180, 0) * transform.rotation;
        }
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb) {
            rb.AddForce(transform.rotation * (Vector3.back * hitForce));
        }
    }

}
