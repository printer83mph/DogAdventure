using System;
using Stims;
using Stims.Receivers;
using UnityEngine;
using UnityEngine.Events;

namespace World.StimListeners
{

    [Serializable]
    public class DeathEvent : UnityEvent<Stim> { }
    
    public class Health : MonoBehaviour
    {
        public DeathEvent OnDeath;

        [SerializeField] private StimReceiver[] receivers = null;
        [SerializeField] private DamageTransformer.Transformer damageTransformer = null;

        [SerializeField] private float maxHealth;
        [SerializeField] private float health;
        [SerializeField] private bool stimAfterDeath;

        private bool _dead = false;

        public bool Dead => _dead;

        private void Start()
        {
            OnDeath ??= new DeathEvent();
            
            _dead = health <= 0;

            if (_dead)
            {
                var stim = new Stim.MysteryDamage(maxHealth);
                _dead = true;
                OnDeath.Invoke(stim);
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

                damageStim.SetDamage(damageTransformer.Evaluate(damageStim));
                Debug.Log("Doing damage: " + damageStim.Damage());

                health -= damageStim.Damage();
                if (health <= 0)
                {
                    // we died
                    _dead = true;
                    OnDeath.Invoke(stim);
                }
            }
        }
    }
}