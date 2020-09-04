using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DistSort : IComparer<EnemyBehaviour> {
    public int Compare(EnemyBehaviour a, EnemyBehaviour b) {
        return a.PlayerDistance.CompareTo(b.PlayerDistance);
    }
}

public class ChadistAI : MonoBehaviour
{

    public delegate void OnSpotDelegate();
    public OnSpotDelegate onSpotDelegate = delegate { };
    
    [Header("Config")]
    public ChadistConfig config;
    public int maxSecurityEngaging = 2;

    [Header("For enemy scripts")]
    public Vector3 lastKnownPos;
    public int alertStatus = 0;
    [HideInInspector]
    public List<EnemyBehaviour> enemyBehaviours = new List<EnemyBehaviour>();

    private float _lastSpot;

    void Update() {
        if (alertStatus != 0) {
            // if we're on the hunt!
            UpdateEngagingEnemies();
            // if it's been long enough then forget abt the player
            if (Time.time - _lastSpot > config.alertLength) {
                alertStatus = 0;
            }
        }
    }

    public void SpotPlayer(Vector3 pos) {
        lastKnownPos = pos;
        alertStatus = 1;
        _lastSpot = Time.time;
        onSpotDelegate();
    }

    private List<EnemyBehaviour> GetSortedEnemies() {
        List<EnemyBehaviour> tempList = new List<EnemyBehaviour>(enemyBehaviours);
        List<EnemyBehaviour> result = new List<EnemyBehaviour>();
        // run through enemies already attacking
        for (int i = tempList.Count - 1; i >= 0; i--) {
            EnemyBehaviour behaviour = tempList[i];
            if (behaviour.Locked) {
                result.Add(behaviour);
                tempList.RemoveAt(i);
            }
        }
        // run through the enemies that can see the player
        for (int i = tempList.Count - 1; i >= 0; i--) {
            EnemyBehaviour behaviour = tempList[i];
            if (behaviour.CanSeePlayer) {
                result.Add(behaviour);
                tempList.RemoveAt(i);
            }
        }
        DistSort s = new DistSort();
        result.Sort(s);
        tempList.Sort(s);
        foreach (EnemyBehaviour behaviour in tempList) {
            result.Add(behaviour);
        }
        return result;
    }

    public void UpdateEngagingEnemies() {
        // TODO: specific enemy types (maybe)
        // TODO: make this more EFFICIENT
        int totalEngaged = 0;
        List<EnemyBehaviour> newList = GetSortedEnemies();
        foreach (EnemyBehaviour behaviour in newList) {
            // foreach (EnemyTypeLimit limit in config.enemyTypeLimits) {
            //     if (limit.type == behaviour.enemyType) {

            //     }
            // }
            bool engage = totalEngaged < config.totalLimit && behaviour.VecToLastKnownPos().sqrMagnitude < Mathf.Pow(config.alertDistance, 2);
            behaviour.SetEngaging(engage);
            if (engage) totalEngaged++;
        }
    }

}
