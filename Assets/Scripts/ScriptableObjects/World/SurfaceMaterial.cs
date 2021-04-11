using System;
using UnityEngine;

namespace ScriptableObjects.World
{
    public enum HitType
    {
        Bullet,
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
        
        [SerializeField] private HitResponse bulletHit;
        [SerializeField] private HitResponse woodBig;
        [SerializeField] private HitResponse woodSmall;
        [SerializeField] private HitResponse metalBig;
        [SerializeField] private HitResponse metalSmall;
        [SerializeField] private HitResponse footstepQuiet;
        [SerializeField] private HitResponse footstepLoud;

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
            Debug.Log("Returning the last working guy");
            return (lastWorking);
        }

        private void OnEnable()
        {
            _responses = new[] {bulletHit, woodBig, woodSmall, metalBig, metalSmall, footstepQuiet, footstepLoud};
        }
    }
}