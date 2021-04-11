using ScriptableObjects.Audio;
using UnityEngine;

namespace ScriptableObjects.Enemies
{
    [CreateAssetMenu(fileName = "EnemyBehaviourConfig", menuName = "ScriptableObjects/Enemies/EnemyBehaviourConfig", order = 0)]
    public class EnemyBehaviourConfig : ScriptableObject
    {
        [SerializeField] private AudioChannel audioChannel = null;

        public AudioChannel AudioChannel => audioChannel;
    }
}