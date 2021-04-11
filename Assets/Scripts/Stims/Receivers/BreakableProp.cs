using Stims;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(StimReceiver))]
public class BreakableProp : MonoBehaviour
{

    public delegate void BreakEvent();
    public BreakEvent onBreak = delegate { };
    
    public float health;
    [FormerlySerializedAs("fxPrefab")] public GameObject breakPrefab;
    
    [FormerlySerializedAs("stimReciever")] [SerializeField] private StimReceiver stimReceiver;

    void Awake()
    {
        stimReceiver.AddStimListener(OnStim);
    }
    
    public void OnStim(Stim stim)
    {
        Debug.Log("Stimmed!");
        if (stim is IStimDamage damageStim)
        {
            health -= (damageStim.Damage());
            Debug.Log("Took " + damageStim.Damage().ToString() + " damage");
            if (health <= 0)
            {
                Break();
            }
        }
    }

    void Break()
    {
        if (breakPrefab) {
            GameObject fx = Instantiate(breakPrefab);
            fx.transform.position = transform.position;
        }
        onBreak();
        Destroy(gameObject);
    }

}
