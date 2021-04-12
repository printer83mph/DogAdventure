using Player;
using ScriptableObjects.Audio.Events;
using UnityEngine;

namespace ScriptableObjects.Audio
{
    // this is just to play audio events from the player!! not an alternative to AudioChannel
    [CreateAssetMenu(fileName = "PlayerAudioChannel", menuName = "ScriptableObjects/Audio/PlayerAudioChannel", order = 0)]
    public class PlayerAudioChannel : ScriptableObject
    {

        private PlayerAudioManager _manager = null;
        private AudioSource _source = null;

        public void SetManager(PlayerAudioManager newManager)
        {
            if (_manager != null)
            {
                Destroy(_source.gameObject);
            }

            _manager = newManager;
            _source = new GameObject().AddComponent<AudioSource>();
            Transform sourceTransform = _source.transform;
            sourceTransform.parent = _manager.transform;
            sourceTransform.localPosition = Vector3.zero;
            sourceTransform.localRotation = Quaternion.identity;
        }

        public void ClearManager()
        {
            _manager = null;
            if (_source == null) return;
            if (_source.gameObject != null) Destroy(_source.gameObject);
            _source = null;
        }
        
        public void PlayEvent(AudioEvent audioEvent)
        {
            audioEvent.Play(_source);
        }

        public void Stop()
        {
            _source.Stop();
        }
        
    }
}