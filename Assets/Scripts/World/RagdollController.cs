using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace World
{
    public class RagdollController : MonoBehaviour
    {

        [SerializeField] private Collider[] colliders;
        [SerializeField] private Rigidbody[] rigidbodies;
        [FormerlySerializedAs("enableRagdoll")] [SerializeField] private bool ragdollEnabled = false;

        public bool RagdollEnabled
        {
            get => ragdollEnabled;

            set
            {
                if (ragdollEnabled == value) return;

                SetRagdollEnabled(value);
                ragdollEnabled = value;
            }
        }

        private void Start()
        {
            SetRagdollEnabled(ragdollEnabled);
        }

        private void SetRagdollEnabled(bool enable)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = enable;
            }
            
            foreach (var rigidbody in rigidbodies)
            {
                rigidbody.isKinematic = !enable;
            }
        }
    }
}
