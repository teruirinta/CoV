using System;
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

    // UI 用
    public float CooldownTimer;
    public float CooldownDuration;

    [Header("Memory Vision")]
    public Volume memoryVolume;
    public VolumeProfile memoryProfile;
    public float fogDencity = 0.8f;

    private float originalFogDensity;
    private bool fogModified = false;

    [Header("各視界データ (ScriptableObject)")]
    public List<VisionData> visionDataList = new List<VisionData>();

    public bool IsTeleporting { get; set; } = false;

    // =============================
    // 初期化
    // =============================
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

        originalFogDensity = RenderSettings.fogDensity;
    }

    // =============================
    // 更新
    // =============================
    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        CooldownTimer = Mathf.Max(cooldownTimer, 0f);
        CooldownDuration = visionCooldown;

        UpdateBatteryUsage();
        UpdateFogForMemoryVision();
    }

    // =============================
    // Fog 制御
    // =============================
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

    // =============================
    // アニメーション・イベント用
    // =============================
    public void SetVision(VisionType vision)
    {
        if (CurrentVision == vision)
            return;

        CurrentVision = vision;
        cooldownTimer = visionCooldown;

        Debug.Log($"👁 Vision 強制切り替え → {vision}");
    }

    // =============================
    // バッテリー消費
    // =============================
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
            SetVision(VisionType.Normal);
            Debug.Log($"⚠ {data.visionName} のバッテリー切れ");
        }
    }

    // =============================
    // データ取得
    // =============================
    public VisionData GetVisionData(VisionType type)
    {
        foreach (var data in visionDataList)
        {
            if (data.visionName.Equals(type.ToString(), StringComparison.OrdinalIgnoreCase))
                return data;
        }
        return null;
    }

    public VisionData GetCurrentVisionData()
    {
        return GetVisionData(CurrentVision);
    }
}