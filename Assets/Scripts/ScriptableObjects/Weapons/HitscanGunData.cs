using ScriptableObjects.Audio;
using ScriptableObjects.World;
using UnityEngine;

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
        [SerializeField] private float kineticPower = 1000f;
        [SerializeField] private LayerMaskConfig layerMask = default;

        [Header("Feedback")]
        [SerializeField] private SurfaceMaterial defaultMaterial = null;
        [SerializeField] private GameObject hitPrefab = null;
        [SerializeField] private AudioEvent audioEvent = null;
        [SerializeField] private PlayerAudioChannel audioChannel = null;

        public float FireRate => fireRate;
        public bool Automatic => automatic;
        public float ReloadTime => reloadTime;
        public int ClipSize => clipSize;
        public AmmoData AmmoData => ammoData;

        public float MaxRange => maxRange;
        public float BaseDamage => baseDamage;
        public float FalloffExponent => falloffExponent;
        public float KineticPower => kineticPower;

        public SurfaceMaterial DefaultMaterial => defaultMaterial;
        public GameObject HitPrefab => hitPrefab;
        public AudioEvent AudioEvent => audioEvent;
        public PlayerAudioChannel AudioChannel => audioChannel;

        public LayerMask LayerMask => layerMask.Mask;
    }
}