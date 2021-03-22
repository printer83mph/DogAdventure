using System;
using ScriptableObjects;
using ScriptableObjects.Audio;
using UnityEngine;

namespace Player
{
    public class PlayerAudioManager : MonoBehaviour
    {
        
        [SerializeField] private PlayerAudioChannel[] audioChannels;
        
        private Transform _audioParent;
        private AudioSource[] _sources;

        public Transform AudioParent => _audioParent;

        private void Awake()
        {
            _audioParent = Camera.main.transform;
        }
        
        private void OnEnable()
        {
            foreach (PlayerAudioChannel audioChannel in audioChannels)
            {
                audioChannel.SetManager(this);
            }
        }

        private void OnDisable()
        {
            foreach (PlayerAudioChannel audioChannel in audioChannels)
            {
                audioChannel.ClearManager();
            }
        }
        
    }
}