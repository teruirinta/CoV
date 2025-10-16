using System.Collections.Generic;
using UnityEngine;

public enum VisionType
{
    Normal,
    NightScope,  // A視界：暗視
    Inverted,    // B視界：上下反転
    Thermal      // C視界：サーモ
}

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance { get; private set; }

    [Header("現在の視界状態")]
    public VisionType CurrentVision { get; private set; } = VisionType.Normal;

    [Header("視界切り替え設定")]
    public float visionCooldown = 3f; // クールダウン時間
    private float cooldownTimer = 0f;

    [Header("各視界データ (ScriptableObject)")]
    public List<VisionData> visionDataList = new List<VisionData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // ✅ ゲーム開始時に全バッテリーを満タンに
        foreach (var data in visionDataList)
        {
            data.currentBattery = data.maxBattery;
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            HandleInput();
        }

        UpdateBatteryUsage();
    }

    void HandleInput()
    {
        // 🎮 Xboxボタン or ⌨️ キー入力対応
        if (Input.GetButtonDown("Fire3") || Input.GetKeyDown(KeyCode.Alpha1)) // B or 1
        {
            TryToggleVision(VisionType.NightScope);
        }
        else if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Alpha2)) // X or 2
        {
            TryToggleVision(VisionType.Inverted);
        }
        else if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Alpha3)) // Y or 3
        {
            TryToggleVision(VisionType.Thermal);
        }
    }

    void TryToggleVision(VisionType vision)
    {
        // バッテリー残量チェック
        var data = GetVisionData(vision);
        if (data != null && data.IsDepleted)
        {
            Debug.LogWarning($"⚠ {data.visionName} のバッテリーが切れています！");
            return;
        }

        // 同じキーで通常視界に戻す
        if (CurrentVision == vision)
            CurrentVision = VisionType.Normal;
        else
            CurrentVision = vision;

        cooldownTimer = visionCooldown;

        Debug.Log($"▶ 現在の視界: {CurrentVision}");
    }

    void UpdateBatteryUsage()
    {
        if (CurrentVision == VisionType.Normal) return;

        var data = GetVisionData(CurrentVision);
        if (data == null) return;

        // バッテリーを減少させる
        data.currentBattery -= data.drainRate * Time.deltaTime;

        if (data.currentBattery <= 0f)
        {
            data.currentBattery = 0f;
            CurrentVision = VisionType.Normal; // バッテリー切れで自動解除
            Debug.Log($"⚠ {data.visionName} のバッテリーが切れました。通常視界に戻ります。");
        }
    }

    public VisionData GetVisionData(VisionType type)
    {
        foreach (var data in visionDataList)
        {
            if (data.visionName.Equals(type.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return data;
        }
        return null;
    }

    public VisionData GetCurrentVisionData()
    {
        return GetVisionData(CurrentVision);
    }


}
