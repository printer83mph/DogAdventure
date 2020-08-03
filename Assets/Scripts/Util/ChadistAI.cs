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
    public OnSpotDelegate onSpotDelegate;
    
    [Header("Config")]
    public float alertLength = 15;
    public int maxEngaging = 999;
    // TODO: max of enemy types should be a dictionary or hash map or some shit
    public int maxSecurityEngaging = 2;
    public int maxChargersEngaging = 1;

    [Header("For enemy scripts")]
    public Vector3 lastKnownPos;
    public int alertStatus = 0;
    public List<EnemyBehaviour> enemyBehaviours = new List<EnemyBehaviour>();

    private float _lastSpot;

    void Update() {
        // if it's been long enough then forget abt the player
        if (Time.time - _lastSpot > alertLength) {
            alertStatus = 0;
        }
        if (alertStatus != 0) {
            // if we're on the hunt!
            UpdateEngagingEnemies();
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
        // run through the enemies that can see the player
        for (int i = tempList.Count - 1; i >= 0; i--) {
            EnemyBehaviour behaviour = tempList[i];
            if (behaviour.CanSeePlayer) {
                result.Add(behaviour);
                tempList.RemoveAt(i);
            }
        }
        tempList.Sort(new DistSort());
        foreach (EnemyBehaviour behaviour in tempList) {
            result.Add(behaviour);
        }
        return result;
    }

    public void UpdateEngagingEnemies() {
        // TODO: go through the sorted list and engage enemies if allowed
        List<EnemyBehaviour> newList = GetSortedEnemies();
        foreach (EnemyBehaviour behaviour in newList) {
            behaviour.SetEngaging(true);
        }
    }

}
