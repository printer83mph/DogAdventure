using Player.Controlling;
using ScriptableObjects.Enemies;
using UnityEngine;

namespace Enemies
{
    public class EnemyVision : MonoBehaviour {

        // inspector vars
        [SerializeField] private EnemyVisionConfig visionConfig = null;
        
        private float _playerVisionWeight = 0;
        public float PlayerVisionWeight => _playerVisionWeight;

        private void Update()
        {
            CheckPlayerVision();
        }

        private void CheckPlayerVision()
        {
            _playerVisionWeight = VisionWeight(PlayerController.Main.transform.position);
        }

        public bool CanSee(Vector3 position, float pullbackDistance = 0)
        {
            var vecToTarget = position - transform.position;
            if (Vector3.Angle(vecToTarget, transform.forward) > visionConfig.MaxAngle ||
                vecToTarget.sqrMagnitude > (visionConfig.MaxDistance * visionConfig.MaxDistance))
                return false;

            // return if the raycast didnt hit anything
            return (!Physics.Raycast(transform.position, vecToTarget,
                Mathf.Max(vecToTarget.magnitude - pullbackDistance, 0), visionConfig.LayerMask));
        }

        // how well can we see the player?
        public float VisionWeight(Vector3 position, bool checkCanSee = true)
        {
            if (!checkCanSee || !CanSee(position)) return 0;

            var vecToTarget = position - transform.position;
            var angleWeight = (Vector3.Angle(vecToTarget, transform.forward) / visionConfig.MaxAngle);
            return 1 / vecToTarget.sqrMagnitude / angleWeight;
        }

        public bool CanCapsule(Vector3 position, float radius)
        {
            if (!CanSee(position)) return false;
        
            Vector3 start = Vector3.MoveTowards(transform.position, position, radius);
            Vector3 end = Vector3.MoveTowards(position, transform.position, radius);
            // may have to disable our own collider for this
            return (!Physics.CheckCapsule(start, end, radius, visionConfig.LayerMask));
        }
    }
}