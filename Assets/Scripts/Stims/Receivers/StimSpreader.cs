using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stims.Receivers
{
    public class StimSpreader : StimReceiver
    {
        private Action<Stim> onStim = delegate {  };
        
        [SerializeField] private List<StimReceiver> receivers;

        public override void SetOnStim(Action<Stim> onStim)
        {
            this.onStim = onStim;
        }

        public override void ClearOnStim()
        {
            onStim = delegate {  };
        }
        
        public override void Stim(Stim stim)
        {
            foreach (var receiver in receivers)
            {
                receiver.Stim(stim);
            }
        }
    }
}