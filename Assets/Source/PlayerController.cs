using Source.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private float _touchTapDelta = 0.45f;

        private PlayerInputMap _playerInputMap;
        private float _xPositionStartTouch;
        private bool _isSwipe;
        
        private void Awake()
        {
            _playerInputMap = new PlayerInputMap();
            _playerInputMap.Enable();
            _playerInputMap.Player.Jump.performed += Jump;
            _playerInputMap.Player.JumpToSegment.performed += JumpToSegment;

            _playerInputMap.Player.PrimaryContact.started += StartTouch;
            _playerInputMap.Player.PrimaryContact.canceled += EndTouch;
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            _xPositionStartTouch = _playerInputMap.Player.PrimaryPosition.ReadValue<Vector2>().x;
        }
        
        private void EndTouch(InputAction.CallbackContext context)
        {
            float delta = _playerInputMap.Player.PrimaryPosition.ReadValue<Vector2>().x - _xPositionStartTouch;

            if (delta < _touchTapDelta && delta > -_touchTapDelta)
            {
                _player.Jump();
                return;
            }
            
            int segmentDelta = delta > 0 ? 1 : -1;
            _player.JumpToSegment(segmentDelta);
        }
        
        private void Jump(InputAction.CallbackContext context)
        {
            _player.Jump();
        }
        
        private void JumpToSegment(InputAction.CallbackContext context)
        {
            int segmentDelta = context.ReadValue<float>() > 0 ? 1 : -1;
            _player.JumpToSegment(segmentDelta);
        }
    }
}