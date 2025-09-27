using TMPro;
using UnityEngine;

public class System_PlayerHealth : MonoBehaviour, System_IDataPersistence
{

    [Header("Player Health")]
    [SerializeField] private int currentPlayerHealth = 100;
    public TextMeshProUGUI healthText;

    public void LoadData(System_GameData data)
    {
        this.currentPlayerHealth = data.currentPlayerHealth;
        UpdateHealthDisplay();
    }

    public void SaveData(ref System_GameData data)
    {
        data.currentPlayerHealth = this.currentPlayerHealth;
    }

    void Start()
    {
        healthText.text = "Health: " + this.currentPlayerHealth;
    }

    public void TakeDamage()
    {
        this.currentPlayerHealth -= 10;
        UpdateHealthDisplay();
    }
    private void UpdateHealthDisplay()
    {
        healthText.text = "Health: " + this.currentPlayerHealth;
    }
}
