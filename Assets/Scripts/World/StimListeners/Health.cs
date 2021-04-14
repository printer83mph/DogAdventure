using System;
using Stims;
using Stims.Receivers;
using UnityEngine;

namespace World.StimListeners
{

    public class Health : MonoBehaviour
    {

        public Action<float> onDamage = delegate {  };
        public Action<IStimDamage> onDeath = delegate {  };

        [SerializeField] private StimReceiver[] receivers;

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
            foreach (var receiver in receivers)
            {
                receiver.SetOnStim(OnStim);
            }
        }

        private void OnDisable()
        {
            foreach (var receiver in receivers)
            {
                receiver.ClearOnStim();
            }
        }

        private void OnStim(Stim stim)
        {
            if (_dead && !stimAfterDeath) return;
            
            if (stim is IStimDamage damageStim)
            {
                
                Debug.Log("Doing damage: " + damageStim.Damage());

                health -= damageStim.Damage();
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