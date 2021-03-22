using ScriptableObjects.Audio;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "HitscanGunData", menuName = "ScriptableObjects/Weapons/HitscanGunData", order = 0)]
    public class HitscanGunData : ScriptableObject
    {
        [SerializeField] private float fireRate = 2.5f;
        [SerializeField] private bool automatic = false;
        [SerializeField] private float reloadTime = .8f;
        [SerializeField] private int clipSize = 7;
        [SerializeField] private AmmoData ammoData = null;

        [SerializeField] private float maxRange = 40f;
        [SerializeField] private float baseDamage = 7f;
        [SerializeField][Range(1, 5)] private float falloffExponent = 2f;
        [SerializeField] private float kineticPower = 1000f;

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
        public AudioEvent AudioEvent => audioEvent;
        public PlayerAudioChannel AudioChannel => audioChannel;
    }
}