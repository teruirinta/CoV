using UnityEngine;

[CreateAssetMenu(menuName = "Vision/VisionData")]
public class VisionData : ScriptableObject
{
    [Header("基本設定")]
    public string visionName = "Normal";

    [Header("バッテリー設定")]
    public float maxBattery = 100f;     // 最大容量
    public float currentBattery = 100f; // 現在容量
    public float drainRate = 5f;        // 1秒あたりの減少量

    public bool IsDepleted => currentBattery <= 0f; // バッテリー切れ判定
}
