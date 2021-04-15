using System;
using System.Collections;
using Player.Controlling;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons
{
    // utility component to pass references to whatever other scripts
    public class Weapon : MonoBehaviour
    {
        private PlayerInventory _inventory;
        private PlayerController _controller;
        private WeaponState _state;
        private PlayerInput _input;
        private bool _equipping;

        [SerializeField] private float equipTime = .3f;
        
        public PlayerInventory Inventory => _inventory;
        public PlayerController Controller => _controller;
        public WeaponState State => _state;
        public PlayerInput Input => _input;
        public bool Equipping => _equipping;

        private void Start()
        {
            StartCoroutine(EquipCoroutine());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Initialize(PlayerInventory inventory, PlayerController controller, WeaponState state, PlayerInput input)
        {
            _inventory = inventory;
            _controller = controller;
            _state = state;
            _input = input;
            
            state.Initialize();
        }
        
        private IEnumerator EquipCoroutine()
        {
            _equipping = true;
            yield return new WaitForSeconds(equipTime);
            _equipping = false;
        }
    }
}