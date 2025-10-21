using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // Drag your WorldSubtitle object here in the Inspector
    public DroppingSubtitle subtitle;

    public void ShowMySubtitle()
    {
        subtitle.StartSubtitle("This is a test subtitle that will fall!");
    }
}