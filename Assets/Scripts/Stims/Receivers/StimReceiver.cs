using System;
using UnityEngine;

namespace Stims
{
    public class StimReceiver : MonoBehaviour
    {
        
        private Action<Stim> _onStim;

        public void SetStimListener(Action<Stim> stimEvent)
        {
            _onStim = stimEvent;
        }

        public void Stim(Stim stim)
        {
            _onStim(stim);
        }

        private void OnCollisionEnter(Collision other)
        {
            Stim(new CollisionStim(other));
        }
    }
}
