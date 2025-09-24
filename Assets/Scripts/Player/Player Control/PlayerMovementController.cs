using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
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

    [Header("Acceleration Settings")]
    public float maxAcceleration = 20f;
    public float maxDeceleration = 20f;
    public float airControl = 0.5f;

    [Header("Friction & Slope Sliding")]
    public float groundFriction = 8f;
    public float slideGravity = 10f;
    public float slopeRayLength = 1.5f;

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
        Vector3 moveDir = cameraTransform.forward * inputDir.y + cameraTransform.right * inputDir.x;
        moveDir.y = 0f;
        moveDir.Normalize();

        float targetSpeed = isCrouching ? crouchSpeed : (input.RunHeld ? runSpeed : walkSpeed);
        Vector3 desiredVelocity = moveDir * targetSpeed;

        // Extract horizontal velocity
        Vector3 currentHorizontal = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 diff = desiredVelocity - currentHorizontal;

        // Accel vs decel
        float accelRate = (desiredVelocity.sqrMagnitude > 0.01f) ? maxAcceleration : maxDeceleration;
        if (!isGrounded) accelRate *= airControl;

        // Apply acceleration
        Vector3 velocityChange = Vector3.ClampMagnitude(diff, accelRate * Time.deltaTime);
        currentHorizontal += velocityChange;

        // Apply ground friction (when no input)
        if (isGrounded && desiredVelocity.sqrMagnitude < 0.01f && currentHorizontal.magnitude > 0f)
        {
            float frictionForce = groundFriction * Time.deltaTime;
            currentHorizontal = Vector3.MoveTowards(currentHorizontal, Vector3.zero, frictionForce);
        }

        // Apply slope sliding
        if (isGrounded && OnSteepSlope(out Vector3 slopeDir))
        {
            currentHorizontal += slopeDir * slideGravity * Time.deltaTime;
        }

        // Recombine with vertical
        velocity.x = currentHorizontal.x;
        velocity.z = currentHorizontal.z;

        // Rotate toward movement direction
        Vector3 lookDir = new Vector3(currentHorizontal.x, 0f, currentHorizontal.z);
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Apply move
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (input.JumpPressed)
        {
            Debug.Log("Jump button pressed.");
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

                if (isCrouching)
                {
                    controller.height = crouchHeight;
                    controller.center = new Vector3(0, crouchHeight / 2f, 0);
                }
                else
                {
                    controller.height = standingHeight;
                    controller.center = new Vector3(0, standingHeight / 2f, 0);
                }
            }
            else
            {
                Debug.Log("Cannot crouch while in the air.");
            }
        }
    }

    private bool OnSteepSlope(out Vector3 slopeDir)
    {
        slopeDir = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayLength))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle > controller.slopeLimit)
            {
                slopeDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                return true;
            }
        }
        return false;
    }
}
