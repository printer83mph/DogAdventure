using System;
using Player.Controlling;
using ScriptableObjects;
using ScriptableObjects.World;
using UnityEngine;
using World;

namespace Player.Aesthetics
{
    public class FootstepPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource source = null;
        [SerializeField] private FootstepManager manager = null;
        [SerializeField] private PlayerController controller = null;
        [SerializeField] private GroundCheck groundCheck = null;

        [SerializeField] private float feetOffset = .1f;
        [SerializeField] private SurfaceMaterial defaultSurfaceMaterial = null;

        private void OnFootstep(int newFoot, bool loud)
        {
            HitType hitType = loud ? HitType.FootstepLoud : HitType.FootstepQuiet;
            
            // self orient with feet + ground
            transform.rotation = controller.Orientation.rotation * groundCheck.GroundRotation;
            Vector3 footPos = transform.TransformPoint(Vector3.right * (feetOffset * newFoot));

            source.transform.position = footPos;

            // figure out what our material is
            SurfaceMaterial material = defaultSurfaceMaterial;
            if (groundCheck.GroundObject)
            {
                WorldProperties properties = groundCheck.GroundObject.GetComponent<WorldProperties>();
                if (properties)
                {
                    material = properties.SurfaceMaterial;
                }
            }

            // instantiate audio + prefab
            var audioEvent = material.GetAudioEvent(hitType);
            if (audioEvent)
                audioEvent.Play(source);
            var prefab = material.GetPrefab(hitType);
            if (prefab)
                Instantiate(prefab, transform.position, transform.rotation);

        }
        
        private void OnEnable()
        {
            manager.onFootstep += OnFootstep;
        }

        private void OnDisable()
        {
            manager.onFootstep -= OnFootstep;
        }
    }
}