using Player.Controlling;
using ScriptableObjects.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons.Player
{
    public class KatanaController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        [SerializeField] private LayerMaskConfig layerMaskConfig;
        
        [SerializeField] private Transform bladeStartPoint;
        [SerializeField] private float katanaLength = .7f;
        [SerializeField] private float swingDuration = .2f;

        private PlayerInventoryWeapon _weapon;
        
        private float _lastSwing = 0;
        private bool _swinging = false;
        
        private static readonly int Swing1 = Animator.StringToHash("swing");

        private void Awake()
        {
            _weapon = GetComponent<PlayerInventoryWeapon>();
        }
        
        private void OnEnable()
        {
            PlayerController.Input.actions["Fire1"].performed += ClickAction;
        }

        private void OnDisable()
        {
            PlayerController.Input.actions["Fire1"].performed -= ClickAction;
        }

        private void Update()
        {
            // draw debug line
            Debug.DrawLine(bladeStartPoint.position, bladeStartPoint.position + bladeStartPoint.rotation * (Vector3.forward * katanaLength), Color.red);

            if (_swinging)
            {
                var katanaRay = new Ray(bladeStartPoint.position, bladeStartPoint.forward);
                if (Physics.Raycast(katanaRay, out RaycastHit hit, katanaLength, layerMaskConfig.Mask))
                {
                    Debug.Log("Hit something!!");
                    // we hit something bro
                    _swinging = false;
                } else if (Time.time - _lastSwing > swingDuration)
                {
                    _swinging = false;
                }
            }
        }

        private void ClickAction(InputAction.CallbackContext ctx)
        {
            if (!_swinging && Time.time - _lastSwing > swingDuration && !_weapon.Equipping)
            {
                Swing();
            }
        }

        private void Swing()
        {
            animator.SetTrigger(Swing1);
            _lastSwing = Time.time;
            _swinging = true;
        }
    }
}
