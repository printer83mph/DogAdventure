using System;
using UnityEngine;

[Serializable]
public class EnemyTypeLimit {
    public string type;
    public int limit;
}

[CreateAssetMenu(fileName="Chadist Config", menuName="ScriptableObjects/ChadistConfig", order=1)]
public class ChadistConfig : ScriptableObject
{
    public float alertLength = 15f;
    public float alertDistance = 35f;
    public int totalLimit = 5;
    [SerializeField]
    public EnemyTypeLimit[] enemyTypeLimits;
}
