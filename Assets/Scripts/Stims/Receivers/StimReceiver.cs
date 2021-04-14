using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stims.Receivers
{
    public abstract class StimReceiver : MonoBehaviour
    {
        // we can receive stims.
        public abstract void Stim(Stim stim);
        public abstract void SetOnStim(Action<Stim> onStim);
        public abstract void ClearOnStim();
    }
}
