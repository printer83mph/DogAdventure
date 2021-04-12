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

        public GameObject InstantiateHitPrefab(HitType hitType, SurfaceMaterial fallback = null, Vector3 position = default, Quaternion rotation = default)
        {
            GameObject prefab = null;
            prefab = GetPrefab(hitType);
            // if still unset and we have fallback
            if (!prefab && fallback)
            {
                prefab = fallback.GetPrefab(hitType);
            }

            if (!prefab) return null;
            return Instantiate(prefab, position, rotation);
        }

        public AudioSource InstantiateAudioEvent(HitType hitType, SurfaceMaterial fallback = null, Vector3 position = default, Transform parent = null)
        {
            AudioEvent audioEvent = null;
            audioEvent = GetAudioEvent(hitType);
            // if still unset and we have fallback
            if (!audioEvent && fallback)
            {
                audioEvent = fallback.GetAudioEvent(hitType);
            }
            
            if (!audioEvent) return null;
            return AudioEvent.InstantiateEvent(audioEvent, position, parent);
        }

        private void OnEnable()
        {
            _responses = new[] {bulletHit, katanaHit, fistsHit, woodBig, woodSmall, metalBig, metalSmall, footstepQuiet, footstepLoud};
        }
    }
}