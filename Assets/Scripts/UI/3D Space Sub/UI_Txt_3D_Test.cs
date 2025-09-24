using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Txt_3D_Test : MonoBehaviour
{
    public UI_Txt_3D_Manager subtitleManager;
    public PlayerInputHandler playerInput;
    public GameInputActions inputActions;
    void Start()
    {
        subtitleManager.ShowSubtitle("Hello World");
    }

    void Update()
    {
        if (playerInput.InteractPressed)
        {
            subtitleManager.ShowSubtitle("You pressed Interact!");
        }
    }
}
