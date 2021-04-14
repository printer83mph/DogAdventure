using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stims.Receivers
{
    public abstract class StimReceiver : MonoBehaviour
    {
        // we can receive stims.
        // called by external components activating stims on us
        public abstract void Stim(Stim stim);
        
        // called by one component tying its action to our receiving
        public abstract void SetOnStim(Action<Stim> onStim);
        public abstract void ClearOnStim();
    }
}
