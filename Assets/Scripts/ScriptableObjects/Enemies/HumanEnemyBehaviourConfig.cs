using ScriptableObjects.Audio;
using UnityEngine;

namespace ScriptableObjects.Enemies
{
    [CreateAssetMenu(fileName = "HumanEnemyBehaviourConfig", menuName = "ScriptableObjects/Enemies/HumanEnemyBehaviourConfig", order = 0)]
    public class HumanEnemyBehaviourConfig : ScriptableObject
    {
        [SerializeField] private AudioChannel audioChannel = null;
        
        [SerializeField] private float idleRadius = 2f;
        [SerializeField] private float idleDelayMin = 1f;
        [SerializeField] private float idleDelayMax = 2f;

        [SerializeField] private float susDispelSpeed = .25f;
        [SerializeField] private float susGainSpeed = 3f;

        public AudioChannel AudioChannel => audioChannel;

        public float IdleRadius => idleRadius;
        public float IdleDelayMin => idleDelayMin;
        public float IdleDelayMax => idleDelayMax;
        public float SusDispelSpeed => susDispelSpeed;
        public float SusGainSpeed => susGainSpeed;
    }
}