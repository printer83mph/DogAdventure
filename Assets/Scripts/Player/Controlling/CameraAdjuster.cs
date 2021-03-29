using UnityEngine;

namespace Player.Controlling
{
    // adjusts camera and viewmodel position
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField] private NewPlayerController controller;
        [SerializeField] private Transform target;
        [SerializeField] private Transform viewmodelTarget;

        private CameraMovement _cameraMovement;

        private void Awake()
        {
            _cameraMovement = controller.CameraMovement;
        }

        private void Update()
        {
            // todo: viewmodel sway, view bob, camera movement extension
        }
    }
}