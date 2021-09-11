using UnityEngine;

namespace ScriptableObjects.Weapons
{
    public class GunKickData : ScriptableObject
    {
        [SerializeField] private float verticalKickMin = 1;
        [SerializeField] private float verticalKickMax = 1;

        [SerializeField] private float horizontalKickMin = -1;
        [SerializeField] private float horizontalKickMax = 1;

        public Vector2 GetKick(float lastShot) =>
            new Vector2(
                Random.Range(horizontalKickMin, horizontalKickMax),
                Random.Range(verticalKickMin, verticalKickMax));
    }
}