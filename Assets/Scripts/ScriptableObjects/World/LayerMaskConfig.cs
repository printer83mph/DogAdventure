using UnityEngine;

namespace ScriptableObjects.World
{
    [CreateAssetMenu(fileName = "Layer Mask Config", menuName = "ScriptableObjects/World/LayerMaskConfig", order = 0)]
    public class LayerMaskConfig : ScriptableObject
    {
        [SerializeField] private LayerMask mask = default;

        public LayerMask Mask => mask;
    }
}