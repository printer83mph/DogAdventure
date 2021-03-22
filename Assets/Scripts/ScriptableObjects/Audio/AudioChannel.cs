using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects.Audio
{
    [CreateAssetMenu(fileName = "AudioChannel", menuName = "ScriptableObjects/Audio/AudioChannel", order = 0)]
    public class AudioChannel : ScriptableObject
    {
        public delegate void PlayAudio(Vector3 pos, SoundType type, float radius);
        public PlayAudio playAudio = delegate {};
    }
}