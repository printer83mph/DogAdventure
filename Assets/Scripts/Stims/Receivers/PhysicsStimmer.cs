﻿using System;
using UnityEngine;

namespace Stims.Receivers
{
    public class PhysicsStimmer : MonoBehaviour
    {
        [SerializeField] private StimReceiver receiver;
        [SerializeField] private bool doDamage = true;
        [SerializeField] private bool doPlayerDamage = false;
        [SerializeField] private float forceDamageThreshold = 30;
        [SerializeField] private float forceToDamageScale = .05f;

        private void OnCollisionEnter(Collision other)
        {
            if (!doDamage)
            {
                receiver.Stim(new CollisionStim(other));
                return;
            }

            Debug.Log("Layer: " + other.collider.gameObject.layer);
            
            if (!doPlayerDamage && other.collider.gameObject.layer == 8) return;
            
            float newDamage = other.impulse.magnitude - forceDamageThreshold;
            receiver.Stim(newDamage > 0
                ? new CollisionDamageStim(other, newDamage * forceToDamageScale)
                : new CollisionStim(other));
        }
    }
}