using System.Collections;
using UnityEngine;

namespace ScriptableObjects.Audio.Events
{
    [CreateAssetMenu(fileName = "ContinuousAudioEvent", menuName = "ScriptableObjects/Audio/ContinuousAudioEvent", order = 0)]
    public class ContinuousAudioEvent : AudioEvent
    {

        [SerializeField] private AudioClip clip = null;
        [SerializeField] private AudioChannel[] channels = null;

        [SerializeField] private float volume = 1f;
        [SerializeField] private float pitch = 1f;

        [SerializeField] private SoundType soundType = SoundType.Unimportant;
        [SerializeField] private float radius = 25;

        private bool _currentlyPlaying;

        public override void Play(AudioSource source)
        {
            if (_currentlyPlaying) return;
            StartPlaying(source);

        }

        private void StartPlaying(AudioSource source)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = true;
            _currentlyPlaying = true;
            source.Play();
        }

        public void Stop(AudioSource source)
        {
            if (!_currentlyPlaying) return;
            _currentlyPlaying = false;
            if (source.clip != clip) return;
            // only stop clip if we've been playing and no one else has changed the clip
            source.Stop();
        }

        IEnumerator AlertChannels(AudioSource source)
        {
            foreach (AudioChannel audioChannel in channels)
            {
                audioChannel.playAudio(source.transform.position, soundType, radius);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}