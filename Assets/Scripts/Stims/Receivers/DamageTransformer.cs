using System;
using UnityEngine;

namespace Stims.Receivers
{
    public class DamageTransformer : StimReceiver
    {

        private Action<Stim> _onStim = delegate {  };
        
        [Serializable]
        private class DamageScaler
        {
            [SerializeField] private DamageType type = default;
            [SerializeField] private float scale = 1;

            public DamageType Type => type;
            public float Scale => scale;
        }

        [Serializable]
        public class Transformer
        {
            [SerializeField] private float baseScale = 1;
            [SerializeField] private DamageScaler[] scalers = null;

            public float Evaluate(IStimDamage stim)
            {
                float scale = baseScale;
                foreach (var scaler in scalers)
                {
                    if (scaler.Type == stim.DamageType())
                    {
                        scale *= scaler.Scale;
                    }
                }

                return stim.Damage() * scale;
            }
        }

        [SerializeField] private Transformer damageTransformer = null;
        
        private Action<Stim> _callBack = delegate {  };

        public override void Stim(Stim stim)
        {
            if (stim is IStimDamage damageStim)
            {
                
                damageStim.SetDamage(damageTransformer.Evaluate(damageStim));
                _onStim.Invoke((Stim) damageStim);
                return;

            }
            
            _onStim.Invoke(stim);

        }

        public override void SetOnStim(Action<Stim> onStim)
        {
            _onStim = onStim;
        }

        public override void ClearOnStim()
        {
            _onStim = delegate {  };
        }
    }
}