using System.Collections.Generic;
using UnityEngine;

public enum VisionType
{
    Normal,
    NightScope,
    Inverted,
    Thermal
}

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance { get; private set; }

    [Header("現在の視界状態")]
    public VisionType CurrentVision { get; private set; } = VisionType.Normal;

    [Header("視界切り替え設定")]
    public float visionCooldown = 3f;
    private float cooldownTimer = 0f;

    // 🔥 外部から読み取り専用でアクセス可能にする
    public float CooldownTimer;     // 現在のクールダウン残り時間
    public float CooldownDuration;  // 最大のクールダウン時間（例：3秒）


    [Header("各視界データ (ScriptableObject)")]
    public List<VisionData> visionDataList = new List<VisionData>();

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
        // クールダウン計算
        cooldownTimer -= Time.deltaTime;

        // ★ BatteryUI用に値を同期
        CooldownTimer = Mathf.Max(cooldownTimer, 0f); // マイナスに行かないように
        CooldownDuration = visionCooldown;

        if (cooldownTimer <= 0f)
        {
            HandleInput();
        }

        UpdateBatteryUsage();
    }


    void HandleInput()
    {
        if (IsTeleporting) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryToggleVision(VisionType.NightScope);
        }
        else if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryToggleVision(VisionType.Inverted);
        }
        else if (Input.GetButtonDown("Fire3") || Input.GetKeyDown(KeyCode.Alpha3))
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
