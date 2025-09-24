using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class DebugStat : MonoBehaviour
{
    public static DebugStat Instance;

    [SerializeField] private TextMeshProUGUI debugText;


    private Dictionary<string, string> stats = new Dictionary<string, string>();
    private StringBuilder debugBuilder = new StringBuilder();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (debugText == null) return;

        debugBuilder.Clear();
        foreach (var stat in stats)
        {
            debugBuilder.AppendLine($"{stat.Key}: {stat.Value}");
        }
        debugText.text = debugBuilder.ToString();
    }

    public void SetStat(string key, string value)
    {
        stats[key] = value;
    }

    public void RemoveStat(string key)
    {
        if (stats.ContainsKey(key))
        stats.Remove(key);
    }
}
