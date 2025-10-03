using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGame_Titanfall : MonoBehaviour
{
    public List<GameObject> pastObject;
    public List<GameObject> presentObject;

    private Player_InputHandle playerInput;

    private bool timeChanged = false;
    [SerializeField] private bool playerInZone = false;
    void Start()
    {
        // Initialize all past objects as inactive and present objects as active at the start
        foreach (GameObject obj in pastObject)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in presentObject)
        {
            obj.SetActive(true);
        }        
    }

    void Update()
    {
        // If we don't have a reference to the player's input yet, try to find it.
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<Player_InputHandle>();

            // If we still couldn't find it (player hasn't spawned), exit the Update for this frame.
            // This prevents errors and we'll try again on the next frame.
            if (playerInput == null)
            {
                return;
            }
        }

        ChangeTime();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    public void ChangeTime()
    {
        if (playerInZone)
        {
            if (playerInput.InteractPressed)
            {
                if (!timeChanged)
                {
                    // Activate all past objects
                    foreach (GameObject obj in pastObject)
                    {
                        obj.SetActive(true);
                    }

                    // Deactivate all present objects
                    foreach (GameObject obj in presentObject)
                    {
                        obj.SetActive(false);
                    }

                    timeChanged = true;
                }
                else
                {
                    // Activate all present objects
                    foreach (GameObject obj in presentObject)
                    {
                        obj.SetActive(true);
                    }

                    // Deactivate all past objects
                    foreach (GameObject obj in pastObject)
                    {
                        obj.SetActive(false);
                    }

                    timeChanged = false;
                }
            }
        }

        return;

    }
}
