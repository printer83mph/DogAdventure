using System;
using Stims.Receivers;
using UnityEngine;
using UnityEngine.Events;

namespace Stims
{

    public class Health : MonoBehaviour
    {

        [Serializable]
        private class DamageTypeScaler
        {
            [SerializeField] private DamageType type;
            [SerializeField] private float scale;

            public DamageType Type => type;
            public float Scale => scale;
        }
        
        public Action<float> onDamage = delegate {  };
        public Action<IStimDamage> onDeath = delegate {  };

        [SerializeField] private StimReceiver receiver;

        [SerializeField] private DamageTypeScaler[] damageScalers = null;

        [SerializeField] private float maxHealth;
        [SerializeField] private float health;
        [SerializeField] private bool stimAfterDeath;

        private bool _dead = true;

        public bool Dead => _dead;

        private void Start()
        {
            _dead = health <= 0;

            if (_dead)
            {
                var stim = new Stim.MysteryDamage(maxHealth);
                _dead = true;
                onDeath.Invoke(stim);
            }
        }

        private void OnEnable()
        {
            receiver.SetOnStim(OnStim);
        }

        private void OnDisable()
        {
            receiver.ClearOnStim();
        }

        private void OnStim(Stim stim)
        {
            if (_dead && !stimAfterDeath) return;
            
            if (stim is IStimDamage damageStim)
            {
                
                Debug.Log("Doing damage: " + damageStim.Damage());
                
                float scale = 1;
                foreach (var scaler in damageScalers)
                {
                    if (scaler.Type == damageStim.DamageType())
                    {
                        scale = scaler.Scale;
                        break;
                    }
                }

                health -= damageStim.Damage() * scale;
                if (health <= 0)
                {
                    // we died
                    _dead = true;
                    onDeath.Invoke(damageStim);
                }
            }
        }
    }
}