using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public Transform cameraTransform;
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeed = 2f;

    private CharacterController controller;
    private PlayerInputHandler input;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Move();
        HandleJump();
        HandleCrouch();
    }

    private void Move()
    {
        Vector2 inputDir = input.MoveInput;
        Vector3 move = cameraTransform.forward * inputDir.y + cameraTransform.right * inputDir.x;
        move.y = 0f;
        move.Normalize();

        float speed = isCrouching ? crouchSpeed : (input.RunHeld ? runSpeed : walkSpeed);

        // Only update horizontal velocity when input exists
        if (move.magnitude > 0.1f)
        {
            velocity.x = move.x * speed;
            velocity.z = move.z * speed;
        }
        else if (isGrounded)
        {
            // If grounded and no input, stop
            velocity.x = 0f;
            velocity.z = 0f;
        }
        // ❌ Do NOT reset horizontal velocity when in the air
        // This way, momentum is preserved mid-air

        // Rotate toward movement direction (on ground or air)
        Vector3 lookDir = new Vector3(velocity.x, 0f, velocity.z);
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Apply combined velocity (XZ + Y)
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (input.JumpPressed)
        {
            if (isGrounded && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if (isCrouching)
            {
                Debug.Log("Cannot jump while crouching.");
            }
            else if (!isGrounded)
            {
                Debug.Log("Cannot jump while in the air.");
            }
        }

        // Gravity always applies
        velocity.y += gravity * Time.deltaTime;

        // Small downward force when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleCrouch()
    {
        if (input.CrouchPressed)
        {
            if (isGrounded)
            {
                isCrouching = !isCrouching;
                controller.height = isCrouching ? crouchHeight : standingHeight;
            }
            else if (!isGrounded)
            {
                Debug.Log("Cannot crouch while in the air.");
            }
        }
    }
}
