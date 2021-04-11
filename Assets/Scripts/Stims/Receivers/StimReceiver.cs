using System;
using UnityEngine;

namespace Stims
{
    public class StimReceiver : MonoBehaviour
    {

        public delegate void OnStimEvent(Stim stim);

        private OnStimEvent onStim = delegate { };

        public void AddStimListener(OnStimEvent stimEvent)
        {
            onStim += stimEvent;
        }

        public void RemoveStimListener(OnStimEvent stimEvent)
        {
            onStim -= stimEvent;
        }
        
        public void Stim(Stim stim)
        {
            onStim(stim);
        }

        private void OnCollisionEnter(Collision other)
        {
            Stim(new CollisionStim(other));
        }
    }
}
