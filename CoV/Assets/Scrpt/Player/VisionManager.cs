using System.Collections.Generic;
using UnityEngine;

public enum VisionType
{
    Normal,
    NightScope, // B視界：暗視
    Inverted,   // Y視界：上下反転
    Thermal     // X視界：サーモ
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

    // ✅ TP中かどうかのフラグ
    public bool IsTeleporting { get; set; } = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
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
        if (IsTeleporting) return; // TP中は視界切り替えを無効化

        // --- Bボタン or キー1：ナイトスコープ視界 ---
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            TryToggleVision(VisionType.NightScope);
        }
        // --- Yボタン or キー2：反転視界 ---
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            TryToggleVision(VisionType.Inverted);
        }
        // --- Xボタン or キー3：サーモ視界 ---
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            TryToggleVision(VisionType.Thermal);
        }
    }

    void TryToggleVision(VisionType vision)
    {
        var data = GetVisionData(vision);

        if (data != null && data.IsDepleted)
        {
            Debug.LogWarning($"⚠ {data.visionName} のバッテリーが切れています！");
            return;
        }

        if (CurrentVision == vision)
            CurrentVision = VisionType.Normal;
        else
            CurrentVision = vision;

        cooldownTimer = visionCooldown;

        Debug.Log($"▶ 現在の視界: {CurrentVision}");
    }

    void UpdateBatteryUsage()
    {
        if (CurrentVision == VisionType.Normal || IsTeleporting)
            return;

        var data = GetVisionData(CurrentVision);
        if (data == null) return;

        data.currentBattery -= data.drainRate * Time.deltaTime;

        if (data.currentBattery <= 0f)
        {
            data.currentBattery = 0f;
            CurrentVision = VisionType.Normal;
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
