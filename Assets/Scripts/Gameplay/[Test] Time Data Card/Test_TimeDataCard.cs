using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Test_TimeDataCard : MonoBehaviour, System_IDataPersistence
{
    [SerializeField] private string timeDataCardID;

    [ContextMenu("Generate GUID for Time Data Card")]
    private void GenerateGuid()
    {
        timeDataCardID = System.Guid.NewGuid().ToString();
    }

    private bool isCardCollected = false;
    private MeshRenderer timeDataCardVisual;
    private Collider timeDataCardCollider;

    private void Awake()
    {
        timeDataCardVisual = GetComponent<MeshRenderer>();
        timeDataCardCollider = GetComponent<Collider>();
    }

    public void LoadData(System_GameData data)
    {
        data.isTimeDataCardCollected.TryGetValue(timeDataCardID, out isCardCollected);
        if (isCardCollected)
        {
            timeDataCardVisual.enabled = false;
            timeDataCardCollider.enabled = false;
        }
    }

    public void SaveData(ref System_GameData data)
    {
        if (data.isTimeDataCardCollected.ContainsKey(timeDataCardID))
        {
            data.isTimeDataCardCollected[timeDataCardID] = isCardCollected;
        }
        else
        {
            data.isTimeDataCardCollected.Add(timeDataCardID, isCardCollected);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCardCollected)
        {
            if (!isCardCollected)
            {
                CollectTimeDataCard();
            }
        }
    }

    private void CollectTimeDataCard()
    {

        isCardCollected = true;
        Debug.Log("Time Data Card Collected! ID: " + timeDataCardID);
        // Here you can add logic to update the player's inventory or game state

        timeDataCardVisual.enabled = false;
        timeDataCardCollider.enabled = false;        
    }
}
