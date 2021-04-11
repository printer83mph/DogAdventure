using ScriptableObjects.World;
using UnityEngine;

namespace World
{
    public class WorldProperties : MonoBehaviour
    {
        [SerializeField] private SurfaceMaterial surfaceMaterial;

        public SurfaceMaterial SurfaceMaterial => surfaceMaterial;
    }
}