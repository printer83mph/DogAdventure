using System;
using ScriptableObjects;
using ScriptableObjects.Audio;
using UnityEngine;

namespace Player
{
    public class PlayerAudioManager : MonoBehaviour
    {
        
        [SerializeField] private PlayerAudioChannel[] audioChannels = null;
        [SerializeField] private Transform audioParent = null;

        public Transform AudioParent => audioParent;

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