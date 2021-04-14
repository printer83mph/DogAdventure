using System;
using UnityEngine;

namespace Stims.Receivers
{
    public class DamageTransformer : StimReceiver
    {

        [Serializable]
        private class DamageTypeScaler
        {
            [SerializeField] private DamageType type = default;
            [SerializeField] private float scale = 1;

            public DamageType Type => type;
            public float Scale => scale;
        }

        [SerializeField] private float baseScale;
        [SerializeField] private DamageTypeScaler[] scalers;
        
        private Action<Stim> _callBack = delegate {  };
        
        public override void Stim(Stim stim)
        {
            if (stim is IStimDamage damageStim)
            {
                float scale = baseScale;
                foreach (var scaler in scalers)
                {
                    if (scaler.Type == damageStim.DamageType())
                    {
                        scale *= scaler.Scale;
                    }
                }
                damageStim.SetDamage(damageStim.Damage() * scale);
                _callBack.Invoke((Stim) damageStim);
                return;
            }

            _callBack.Invoke(stim);
        }

        // health is probably asking
        public override void SetOnStim(Action<Stim> onStim)
        {
            _callBack = onStim;
        }

        public override void ClearOnStim()
        {
            _callBack = delegate {  };
        }
    }
}