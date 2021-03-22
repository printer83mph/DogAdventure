using Player;
using UnityEngine;

namespace ScriptableObjects.Audio
{
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
            _source.transform.parent = _manager.AudioParent;
            _source.transform.localPosition = Vector3.zero;
            _source.transform.localRotation = Quaternion.identity;
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