using UnityEngine;

public class Game_QTENeedle : MonoBehaviour
{
    // A reference to the main controller script
    public Game_QTEController qteController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check the tag of the object we entered
        //if (other.CompareTag("GreatSuccessZone"))
        //{
        //    qteController.SetCurrentZone(QTEZone.GreatSuccess);
        //}
        if (other.CompareTag("Coll_2D_SuccessZone"))
        {
            qteController.SetCurrentZone(QTEZone.Success);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // When we exit any zone, reset the status to None
        qteController.SetCurrentZone(QTEZone.None);
    }
}