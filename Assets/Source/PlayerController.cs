using UnityEngine;
using UnityEngine.InputSystem;

namespace Source
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player _player;

        private PlayerInputMap _playerInputMap;
        
        private void Awake()
        {
            _playerInputMap = new PlayerInputMap();
            _playerInputMap.Enable();
            _playerInputMap.Player.Jump.performed += Jump;
            _playerInputMap.Player.JumpToSegment.performed += JumpToSegment;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            _player.Jump();
        }
        
        private void JumpToSegment(InputAction.CallbackContext context)
        {
            int segmentDelta = (int)context.ReadValue<float>();
            _player.JumpToSegment(segmentDelta);
        }
    }
}