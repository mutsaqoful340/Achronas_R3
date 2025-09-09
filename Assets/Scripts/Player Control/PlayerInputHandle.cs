using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool RunHeld { get; private set; }
    public bool CrouchPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    public GameInputActions inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new GameInputActions();

            inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

            inputActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

            inputActions.Player.Jump.performed += ctx => JumpPressed = true;
            inputActions.Player.Run.performed += ctx => RunHeld = true;
            inputActions.Player.Run.canceled += ctx => RunHeld = false;
            inputActions.Player.Crouch.performed += ctx => CrouchPressed = true;

            inputActions.Player.Interact.performed += ctx => InteractPressed = true;
            inputActions.Player.Interact.canceled += ctx => InteractPressed = false;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void LateUpdate()
    {
        // Reset one-frame buttons
        JumpPressed = false;
        CrouchPressed = false;
        InteractPressed = false;
    }
}
