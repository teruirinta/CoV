using UnityEngine;
using UnityEngine.UI;

public class SaltStockUI : MonoBehaviour
{
    [Header("UI")]
    public Text stockText;

    [Header("éQè∆")]
    public BallThrower ballThrower;

    void Start()
    {
        UpdateStockText();
    }

    void Update()
    {
        UpdateStockText();
    }

    void UpdateStockText()
    {
        if (ballThrower == null || stockText == null) return;

        stockText.text = $"Å~{ballThrower.CurrentStock}";
    }
}