using UnityEngine;

public class SimpleTriggerTest : MonoBehaviour
{
    // This will be called by Unity's physics engine if a trigger collision happens.
    void OnTriggerEnter(Collider other)
    {
        // We use LogError to make the message stand out in red.
        Debug.LogError("--- TRIGGER SUCCESS on '" + this.gameObject.name 
            + "'! The object that entered was: '" + other.gameObject.name + "'.");
    }
}