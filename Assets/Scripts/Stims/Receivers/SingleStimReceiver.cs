using System;

namespace Stims.Receivers
{
    public class SingleStimReceiver : StimReceiver
    {
        
        private Action<Stim> _onStim = delegate { };

        public override void SetOnStim(Action<Stim> onStim)
        {
            _onStim = onStim;
        }

        public override void ClearOnStim()
        {
            _onStim = delegate {  };
        }
        
        public override void Stim(Stim stim)
        {
            _onStim?.Invoke(stim);
        }
    }
}