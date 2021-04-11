using System;
using Stims;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    // delegate shit
    public delegate void OnDeathEvent(Stim stim);
    public OnDeathEvent onDeath = delegate { };

    public float Health => _health;
    public bool Dead => _dead;

    // inspector
    [SerializeField]
    private float _health = 15;

    // auto-assigned
    private StimReceiver _stimReceiver;
    
    // mathstuff
    private bool _dead;

    private void Awake()
    {
        _stimReceiver = GetComponent<StimReceiver>();
    }

    private void OnEnable()
    {
        _stimReceiver.AddStimListener(OnDamage);
    }

    private void OnDisable()
    {
        _stimReceiver.RemoveStimListener(OnDamage);
    }

    private void OnDamage(Stim stim) {
        if (_dead) return;
        if (stim is IStimDamage damageStim)
        {
            Damage(damageStim);
        }
    }

    public void Damage(IStimDamage stim) {
        if (_dead) return;
        _health -= stim.Damage();
        if (_health <= 0) {
            Debug.Log("Bro.. i died");
            _dead = true;
            onDeath((Stim)stim);
        }
    }

}