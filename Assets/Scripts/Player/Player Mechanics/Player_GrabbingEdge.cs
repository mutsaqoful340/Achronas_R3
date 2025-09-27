using UnityEngine;
using UnityEngine.InputSystem;

public class Player_GrabbingEdge : MonoBehaviour
{
    [SerializeField] private bool isTouchingEdge = false;
    [SerializeField] private bool isGrabbingEdge = false;
    private Player_InputHandle playerInput;
    private Player_MovementController PlayerMovementController;

    private void Awake()
    {
        PlayerMovementController = GetComponent<Player_MovementController>();
        playerInput = GetComponent<Player_InputHandle>();
    }

    //    private void OnEnable()
    //    {
    //        playerInput.inputActions.Player.Interact.performed += OnInteract;
    //    }

    //    private void OnDisable()
    //    {
    //        playerInput.inputActions.Player.Interact.performed -= OnInteract;
    //    }
    private void Update()
    {
        HandleGrab();
    }

    private void HandleGrab()
    {
        if (playerInput.InteractPressed)
        {
            Debug.Log("Interact button pressed.");
            if (!isTouchingEdge)
            {
                Debug.Log("Player is not touching an edge.");
                return;
            }

            if (!isGrabbingEdge)
            {
                isGrabbingEdge = true;
                GrabEdge();
            }
            else
            {
                isGrabbingEdge = false;
                ReleaseEdge();
            }
        }

    }

    private void GrabEdge()
    {
        PlayerMovementController.enabled = false;
        Debug.Log("Player is now grabbing the edge.");
    }

    private void ReleaseEdge()
    {
        PlayerMovementController.enabled = true;
        Debug.Log("Player has released the edge.");
    }

    private void OnTriggerEnter(Collider EdgeWallCollider)
    {
        if (EdgeWallCollider.CompareTag("EdgeWall"))
        {
            isTouchingEdge = true;
            Debug.Log("Player is touching the wall edge.");
        }
    }

    private void OnTriggerExit(Collider EdgeWallCollider)
    {
        if (EdgeWallCollider.CompareTag("EdgeWall"))
        {
            isTouchingEdge = false;
            Debug.Log("Player is no longer touching the wall edge.");
        }
    }
}
