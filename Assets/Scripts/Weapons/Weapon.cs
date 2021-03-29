using Player.Controlling;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        private PlayerInventory _inventory;
        private NewPlayerController _controller;
        private WeaponInventoryState _inventoryState;
        private PlayerInput _input;
        
        public PlayerInventory Inventory => _inventory;
        public NewPlayerController Controller => _controller;
        public WeaponInventoryState InventoryState => _inventoryState;
        public PlayerInput Input => _input;

        public void Initialize(PlayerInventory inventory, NewPlayerController controller, WeaponInventoryState inventoryState, PlayerInput input)
        {
            _inventory = inventory;
            _controller = controller;
            _inventoryState = inventoryState;
            _input = input;
        }
    }
}