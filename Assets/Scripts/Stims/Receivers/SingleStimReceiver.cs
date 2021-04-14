using System;

namespace Stims.Receivers
{
    public class SingleStimReceiver : StimReceiver
    {
        
        public Action<Stim> onStim = delegate { };

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
            onStim?.Invoke(stim);
        }
    }
}