using ScriptableObjects.Audio;
using ScriptableObjects.Audio.Events;
using ScriptableObjects.World;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "HitscanGunData", menuName = "ScriptableObjects/Weapons/HitscanGunData", order = 0)]
    public class HitscanGunData : ScriptableObject
    {
        [Header("Handling")]
        [SerializeField] private float fireRate = 2.5f;
        [SerializeField] private bool automatic = false;
        [SerializeField] private float reloadTime = .8f;
        [SerializeField] private int clipSize = 7;
        [SerializeField] private AmmoData ammoData = null;

        [Header("Damage")]
        [SerializeField] private float maxRange = 40f;
        [SerializeField] private float baseDamage = 7f;
        [SerializeField][Range(1, 5)] private float falloffExponent = 2f;
        [FormerlySerializedAs("kineticPower")] [SerializeField] private float baseForce = 1000f;

        [Header("Feedback")]
        [SerializeField] private AudioEvent audioEvent = null;
        [SerializeField] private PlayerAudioChannel audioChannel = null;
        
        [Header("Overrides")]
        [SerializeField] private GameObject hitPrefab = null;
        [SerializeField] private AudioEvent hitAudioEvent = null;

        public float FireRate => fireRate;
        public bool Automatic => automatic;
        public float ReloadTime => reloadTime;
        public int ClipSize => clipSize;
        public AmmoData AmmoData => ammoData;

        public float MaxRange => maxRange;
        public float BaseDamage => baseDamage;
        public float FalloffExponent => falloffExponent;
        public float BaseForce => baseForce;

        public AudioEvent AudioEvent => audioEvent;
        public PlayerAudioChannel AudioChannel => audioChannel;
        
        public GameObject HitPrefab => hitPrefab;
        public AudioEvent HitAudioEvent => hitAudioEvent;
    }
}