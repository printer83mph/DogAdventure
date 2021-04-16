﻿using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapons/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string displayName = null;
        // whatever other weapon data stuff we need
        [FormerlySerializedAs("prefab")] [SerializeField] private GameObject playerPrefab = null;
        public GameObject PlayerPrefab => playerPrefab;

        [SerializeField] private GameObject enemyPrefab = null;
        public GameObject EnemyPrefab => enemyPrefab;
    }
}