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

        // クールタイムが終わっていれば入力を受け付ける
        if (cooldownTimer <= 0f)
        {
            HandleInput();
        }

        UpdateBatteryUsage();
        UpdateFogForMemoryVision();
    }

    // =============================
    // ★ 追加：キー入力処理
    // =============================
    void HandleInput()
    {
        if (IsTeleporting) return;

        // 1キー: ナイトスコープ
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetButtonDown("Fire1"))
        {
            ToggleVision(VisionType.NightScope);
        }
        // 2キー: 反転
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetButtonDown("Fire2"))
        {
            ToggleVision(VisionType.Inverted);
        }
        // 3キー: 記憶メガネ
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetButtonDown("Fire3"))
        {
            ToggleVision(VisionType.MemoryVision);
        }
    }

    // =============================
    // ★ 追加：ON/OFFを切り替えるロジック
    // =============================
    void ToggleVision(VisionType vision)
    {
        // 切り替え先のバッテリーをチェック
        var data = GetVisionData(vision);
        if (data != null && data.currentBattery <= 0f)
        {
            Debug.LogWarning($"⚠ {vision} はバッテリー不足です");
            return;
        }

        // 同じ視界をもう一度押したら Normal に戻す
        VisionType nextVision = (CurrentVision == vision) ? VisionType.Normal : vision;
        SetVision(nextVision);
    }

    // =============================
    // 視界の確定切り替え
    // =============================
    public void SetVision(VisionType vision)
    {
        if (CurrentVision == vision) return;

        CurrentVision = vision;
        cooldownTimer = visionCooldown; // クールタイム開始

        Debug.Log($"👁 Vision 切り替え → {vision}");
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