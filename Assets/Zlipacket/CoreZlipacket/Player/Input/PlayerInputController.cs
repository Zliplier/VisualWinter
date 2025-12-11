using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Player.Input
{
    public class PlayerInputController : Singleton<PlayerInputController>
    {
        private InputSystem_Actions _inputSystem;
        private InputAction _move;
        private InputAction _jump;
        private InputAction _enter;
        private InputAction _leftMouse;
        private InputAction _scrollMouse;
        private InputAction _interact;
        private InputAction _mousePosition;

        public UnityEvent onMouseLeftDown;
        public UnityEvent onMouseLeftUp;
        public UnityEvent onJump;
        public UnityEvent onEnter;
        public UnityEvent onMouseScroll;
        public UnityEvent onInteract;
        
        public Vector2 MousePosition { get; private set; }
        public Vector2 MovementInputVector {get; private set;}
        public bool IsLeftMouseDown {get; private set;}
        public float MouseScroll {get; private set;}

        public override void Awake()
        {
            base.Awake();
            _inputSystem = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _move = _inputSystem.Player.Move;
            _move.Enable();
            _move.performed += MovementInput;
            _move.canceled += MovementInput;
            
            _jump = _inputSystem.Player.Jump;
            _jump.Enable();
            _jump.performed += JumpInput;
            
            _enter = _inputSystem.Player.Return;
            _enter.Enable();
            _enter.performed += EnterInput;

            _leftMouse = _inputSystem.Player.Attack;
            _leftMouse.Enable();
            _leftMouse.started += LeftMouseDownInput;
            _leftMouse.canceled += LeftMouseUpInput;

            _scrollMouse = _inputSystem.Player.MouseWheel;
            _scrollMouse.Enable();
            _scrollMouse.performed += ScrollMouseInput;
            _scrollMouse.canceled += ScrollMouseInput;
            
            _interact = _inputSystem.Player.Interact;
            _interact.Enable();
            _interact.started += InteractInput;
            
            _mousePosition = _inputSystem.Player.MousePosition;
            _mousePosition.Enable();
            _mousePosition.performed += MousePositionInput;
        }

        private void OnDisable()
        {
            _move.performed -= MovementInput;
            _move.canceled -= MovementInput;
            _move.Disable();
            
            _jump.performed -= JumpInput;
            _jump.Disable();
            
            _enter.performed -= EnterInput;
            _enter.Disable();
            
            _leftMouse.started -= LeftMouseDownInput;
            _leftMouse.canceled -= LeftMouseUpInput;
            _leftMouse.Disable();
            
            _scrollMouse.performed -= ScrollMouseInput;
            _scrollMouse.canceled -= ScrollMouseInput;
            _scrollMouse.Disable();
            
            _interact.started -= InteractInput;
            _interact.Disable();
            
            _mousePosition.performed -= MousePositionInput;
            _mousePosition.Disable();
        }

        private void MovementInput(InputAction.CallbackContext context)
        {
            MovementInputVector = context.ReadValue<Vector2>();
        }
        
        private void LeftMouseDownInput(InputAction.CallbackContext context)
        {
            IsLeftMouseDown = context.ReadValueAsButton();
            onMouseLeftDown?.Invoke();
        }
        
        private void JumpInput(InputAction.CallbackContext context)
        {
            onJump?.Invoke();
        }

        private void EnterInput(InputAction.CallbackContext context)
        {
            onEnter?.Invoke();
        }

        private void LeftMouseUpInput(InputAction.CallbackContext context)
        {
            IsLeftMouseDown = context.ReadValueAsButton();
            onMouseLeftUp?.Invoke();
        }
        
        private void ScrollMouseInput(InputAction.CallbackContext context)
        {
            MouseScroll = context.ReadValue<float>();
            onMouseScroll?.Invoke();
        }

        private void InteractInput(InputAction.CallbackContext context)
        {
            onInteract?.Invoke();
        }
        
        private void MousePositionInput(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }
    }
}