using System;
using ScriptableObjects.Audio.Events;
using UnityEngine;

namespace ScriptableObjects.World
{
    public enum HitType
    {
        Bullet,
        Katana,
        Fists,
        WoodBig,
        WoodSmall,
        MetalBig,
        MetalSmall,
        FootstepQuiet,
        FootstepLoud
    }
    
    [CreateAssetMenu(fileName = "Surface Material", menuName = "ScriptableObjects/World/SurfaceMaterial", order = 0)]
    public class SurfaceMaterial : ScriptableObject
    {

        [SerializeField] private SurfaceMaterial parent = null;

        [Serializable]
        public class HitResponse
        {
            [SerializeField] private GameObject prefab = null;
            [SerializeField] private AudioEvent audioEvent = null;

            public GameObject Prefab => prefab;
            public AudioEvent AudioEvent => audioEvent;
        }
        
        [SerializeField] private HitResponse bulletHit = null;
        [SerializeField] private HitResponse katanaHit = null;
        [SerializeField] private HitResponse fistsHit = null;
        [SerializeField] private HitResponse woodBig = null;
        [SerializeField] private HitResponse woodSmall = null;
        [SerializeField] private HitResponse metalBig = null;
        [SerializeField] private HitResponse metalSmall = null;
        [SerializeField] private HitResponse footstepQuiet = null;
        [SerializeField] private HitResponse footstepLoud = null;

        private HitResponse[] _responses = null;

        public GameObject GetPrefab(HitType hitType, GameObject lastWorking = null)
        {
            var currentPrefab = _responses[(int) hitType].Prefab;
            if (currentPrefab) lastWorking = currentPrefab;
            // if parent, keep recursing bro
            if (parent) return parent.GetPrefab(hitType, lastWorking);
            // if no parent just return the last working guy
            return (lastWorking);
        }
        
        public AudioEvent GetAudioEvent(HitType hitType, AudioEvent lastWorking = null)
        {
            var currentAudioEvent = _responses[(int) hitType].AudioEvent;
            if (currentAudioEvent) lastWorking = currentAudioEvent;
            // if parent, keep recursing bro
            if (parent) return parent.GetAudioEvent(hitType, lastWorking);
            // if no parent just return the last working guy
            return (lastWorking);
        }

        public static void InstantiateEffects(SurfaceMaterial material, HitType hitType, Vector3 position,
            Quaternion rotation = default, Transform prefabParent = null, Transform audioParent = null, SurfaceMaterial fallback = null)
        {
            InstantiateHitPrefab(material, hitType, position, rotation, prefabParent, fallback);
            InstantiateAudioEvent(material, hitType, position, audioParent, fallback);
        }
        
        // uses fallback if no surface material defined
        public static GameObject InstantiateHitPrefab(SurfaceMaterial material, HitType hitType, Vector3 position,
            Quaternion rotation = default, Transform parent = null, SurfaceMaterial fallback = null)
        {
            GameObject prefab = null;
            if (material)
            {
                prefab = material.GetPrefab(hitType);
            }
            else if (fallback)
            {
                prefab = fallback.GetPrefab(hitType);
            }

            return prefab
                ? Instantiate(prefab, position, rotation, parent)
                : null;
        }

        public static AudioSource InstantiateAudioEvent(SurfaceMaterial material, HitType hitType, Vector3 position,
            Transform parent = null, SurfaceMaterial fallback = null)
        {
            AudioEvent audioEvent = null;
            if (material)
            {
                audioEvent = material.GetAudioEvent(hitType);
            }
            else if (fallback)
            {
                audioEvent = fallback.GetAudioEvent(hitType);
            }
            return AudioEvent.InstantiateEvent(audioEvent, position, parent);
        }

        private void OnEnable()
        {
            _responses = new[] {bulletHit, katanaHit, fistsHit, woodBig, woodSmall, metalBig, metalSmall, footstepQuiet, footstepLoud};
        }
    }
}