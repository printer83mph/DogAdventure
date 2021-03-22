using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class DistSort : IComparer<EnemyBehaviour> {
    public int Compare(EnemyBehaviour a, EnemyBehaviour b) {
        return a.LastKnownPosDistance.CompareTo(b.LastKnownPosDistance);
    }
}

public class ChadistAI : MonoBehaviour
{

    public int maxEngagingCount = 3;
    public List<EnemyBehaviour> enemies;
    public List<EnemyBehaviour> engaging;

    public int totalEnemies => enemies.Count;
    public int totalEngaging => engaging.Count;
    public bool MaxEngaging => engaging.Count >= maxEngagingCount;

    private void Start()
    {
        StartCoroutine(nameof(UpdateAI));
    }

    IEnumerator UpdateAI()
    {
        while (true)
        {
            CullEngagingEnemies();
            yield return new WaitForSeconds(1);
        }
    }

    public void CullEngagingEnemies()
    {
        // nothing to do if value is within bounds
        if (totalEngaging <= maxEngagingCount) return;
        // sort by distance
        engaging.Sort(new DistSort());
        // filter out overriding dudes
        List<EnemyBehaviour> filtered = engaging.Where( behaviour => !behaviour.OverrideEngageLimit ).ToList();
        // return if we have no enemies we can remove
        if (filtered.Count == 0) return;
        while (totalEngaging > maxEngagingCount)
        {
            Debug.Log("Removing an extra enemy from engaging list");
            EnemyBehaviour behaviour = filtered[filtered.Count - 1];
            behaviour.Disengage();
        }
    }
}

public enum SoundType
{
    Unimportant,
    Suspicious,
    Alarming,
    Stunning
}
