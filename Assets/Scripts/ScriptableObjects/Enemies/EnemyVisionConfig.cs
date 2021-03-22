using ScriptableObjects.Audio;
using UnityEngine;

namespace ScriptableObjects.Enemies
{
    [CreateAssetMenu(fileName = "EnemyVisionConfig", menuName = "ScriptableObjects/Enemies/EnemyVisionConfig", order = 0)]
    public class EnemyVisionConfig : ScriptableObject
    {
        // vision
        [SerializeField] private LayerMask layerMask = (1 << 0) | (1 << 9);
        [SerializeField] private float maxDistance = 25;
        [SerializeField] private float maxAngle = 85;
        // TODO: make los radius only affect whether or not we can attack yet
        [SerializeField] private float losRadius = 0;

        public LayerMask LayerMask => layerMask;
        public float MaxDistance => maxDistance;
        public float MaxAngle => maxAngle;
        public float LosRadius => losRadius;
    }
}