using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TimeDataCard_Collected_Text : MonoBehaviour
{
    [SerializeField] private int collectedTimeDataCards = 0;

    private TextMeshProUGUI timeDataCardCollectedText;

    private void Awake()
    {
        timeDataCardCollectedText = GetComponent<TextMeshProUGUI>();
    }
    
    public void LoadData(System_GameData data)
    {
        foreach(KeyValuePair<string, bool> pair in data.isTimeDataCardCollected)
        {
            if (pair.Value)
            {
                collectedTimeDataCards++;
            }
        }
    }

    public void SaveData(ref System_GameData data)
    {
        
    }

    private void Start()
    {

    }

    private void Update()
    {
        timeDataCardCollectedText.text = "Time Data Cards Collected: " + collectedTimeDataCards;
    }
}
