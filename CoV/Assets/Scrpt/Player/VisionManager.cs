using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum VisionType
{
    Normal,
    NightScope,
    Inverted,
    MemoryVision
}

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance { get; private set; }

    [Header("現在の視界状態")]
    public VisionType CurrentVision { get; private set; } = VisionType.Normal;

    [Header("視界切り替え設定")]
    public float visionCooldown = 3f;
    private float cooldownTimer = 0f;

    // ★ BatteryUI用
    public float CooldownTimer;
    public float CooldownDuration;

    [Header("Memory Vision")]
    public Volume memoryVolume;
    public VolumeProfile memoryProfile;
    public float fogDencity = 0.8f;  // Fog 30%軽減 → density を 0.7倍にする

    private float originalFogDensity;   // ← Fog 初期値保存用
    private bool fogModified = false;   // ← 連続で書き換えないように制御

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

        // Fog の初期値を記録
        originalFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        CooldownTimer = Mathf.Max(cooldownTimer, 0f);
        CooldownDuration = visionCooldown;

        if (cooldownTimer <= 0f)
        {
            HandleInput();
        }

        UpdateBatteryUsage();
        UpdateFogForMemoryVision();  // ★ Fog処理を毎フレーム実行
    }

    // ============================
    //      Fog の制御
    // ============================
    void UpdateFogForMemoryVision()
    {
        if (CurrentVision == VisionType.MemoryVision)
        {
            if (!fogModified)
            {
                fogModified = true;
                RenderSettings.fogDensity = originalFogDensity * fogDencity;
            }
        }
        else
        {
            if (fogModified)
            {
                fogModified = false;
                RenderSettings.fogDensity = originalFogDensity;
            }
        }
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
            TryToggleVision(VisionType.MemoryVision);
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
