using UnityEngine;
using UnityEngine.InputSystem;

namespace RIEVES.GGJ2026.Runtime.Movement
{
    internal sealed class ActionInputProvider : InputProvider
    {
        [SerializeField]
        private InputActionReference moveAction;

        private Vector2 moveAxis;

        public override Vector2 MoveAxis => moveAxis;

        private void OnEnable()
        {
            moveAction.action.performed += OnMovePerformed;
            moveAction.action.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            moveAction.action.performed -= OnMovePerformed;
            moveAction.action.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            moveAxis = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext obj)
        {
            moveAxis = Vector2.zero;
        }
    }
}
