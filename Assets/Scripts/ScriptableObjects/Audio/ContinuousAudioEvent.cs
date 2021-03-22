using System.Collections;
using UnityEngine;

namespace ScriptableObjects.Audio
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

        public override void Play(AudioSource source)
        {
            if (source.isPlaying) return;
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = true;
            // todo: figure out how to regularly alert
            // StartCoroutine(AlertChannels(source));
            source.Play();
        }

        public void Stop(AudioSource source)
        {
            if (source.isPlaying) source.Stop();
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