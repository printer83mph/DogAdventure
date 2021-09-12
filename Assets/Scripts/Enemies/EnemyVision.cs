using Player.Controlling;
using ScriptableObjects.Enemies;
using UnityEngine;

namespace Enemies
{
    public class EnemyVision : MonoBehaviour {

        // inspector vars

        [SerializeField] private EnemyVisionConfig visionConfig = null;

        public bool InVisionCone(Vector3 position)
        {
            var relativePos = transform.InverseTransformPoint(position);
            return !(Vector3.Angle(relativePos, Vector3.forward) > visionConfig.MaxAngle ||
                     relativePos.magnitude > visionConfig.MaxDistance);
        }

        public bool CanSee(Vector3 position, float pullbackDistance = 0)
        {
            if (!InVisionCone(position)) return false;

            Vector3 vecToPos = position - transform.position;
            // return if the raycast didnt hit anything
            return (!Physics.Raycast(transform.position, vecToPos, vecToPos.magnitude - pullbackDistance, visionConfig.LayerMask));
        }

        public bool CanSeePlayer() => (PlayerController.Main && CanSee(PlayerController.Main.Orientation.position));

        public bool CanCapsule(Vector3 position, float radius)
        {
            if (!InVisionCone(position)) return false;
        
            Vector3 start = Vector3.MoveTowards(transform.position, position, radius);
            Vector3 end = Vector3.MoveTowards(position, transform.position, radius);
            // may have to disable our own collider for this
            return (!Physics.CheckCapsule(start, end, radius, visionConfig.LayerMask));
        }
    }
}