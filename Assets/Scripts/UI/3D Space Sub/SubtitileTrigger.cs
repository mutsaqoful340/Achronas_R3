using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    public FallingSubtitle subtitle; // Drag your subtitle object here in the Inspector
    [TextArea(3, 10)]
    public string textToShow;

    void Start()
    {
        if (subtitle != null)
        {
            // Call the function to start the animation
            subtitle.DisplayText(textToShow);
        }
    }
}