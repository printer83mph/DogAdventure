using UnityEngine;

namespace ScriptableObjects.Audio.Events
{
    [CreateAssetMenu(fileName = "VariedAudioEvent", menuName = "ScriptableObjects/Audio/VariedAudioEvent", order = 0)]
    public class VariedAudioEvent : AudioEvent
    {
        [SerializeField] private AudioClip[] clips = null;
        [SerializeField] private AudioChannel[] channels = null;
        
        [SerializeField] private float volumeMin = 1;
        [SerializeField] private float volumeMax = 1;
        [SerializeField] private float pitchMin = 1;
        [SerializeField] private float pitchMax = 1;
        
        [SerializeField] private SoundType soundType = SoundType.Unimportant;
        [SerializeField] private float radius = 25;
        [SerializeField] private bool actuallyRandom = true;
        
        private int _lastClip;
        
        public SoundType SoundType => soundType;

        private AudioClip GetRandomClip()
        {
            int index = Random.Range(0, clips.Length - 1);
            if (index >= _lastClip)
            {
                index++;
            }

            _lastClip = index;
            return clips[index];
        }

        private AudioClip GetActualRandomClip()
        {
            return clips[Random.Range(0, clips.Length)];
        }

        public AudioClip GetClip()
        {
            if (clips == null) return null;
            if (clips.Length == 1)
            {
                return clips[0];
            }
            return actuallyRandom ? GetActualRandomClip() : GetRandomClip();
        }

        public override void Play(AudioSource source)
        {
            PlayAudio(source);
            TriggerChannels(source.transform.position);            
        }
        
        public void PlayAudio(AudioSource source)
        {
            source.loop = false;
            source.clip = GetClip();
            source.volume = Random.Range(volumeMin, volumeMax);
            source.pitch = Random.Range(pitchMin, pitchMax);
            source.Play();
        }

        public void TriggerChannels(Vector3 position)
        {
            foreach (AudioChannel channel in channels)
            {
                channel.playAudio(position, soundType, radius);
                // Debug.Log("Played audio");
            }
        }
    }
}