using UnityEngine;

public enum VisionType
{
    Normal,      // 通常視界
    NightScope,  // A視界：暗視
    Inverted,    // B視界：上下反転
    Thermal      // C視界：サーモ
}

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance { get; private set; }

    [Header("視界データ設定（ScriptableObject）")]
    public VisionData normalVision;
    public VisionData nightScopeVision;
    public VisionData invertedVision;
    public VisionData thermalVision;

    [Header("クールダウン設定")]
    public float visionCooldown = 0.5f; // 連続切り替え防止用

    private float cooldownTimer = 0f;
    private VisionType currentVision = VisionType.Normal;

    public VisionType CurrentVision => currentVision;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        // ゲーム開始時に全視界のバッテリーを満タンに
        ResetAllBatteries();
    }

    void ResetAllBatteries()
    {
        VisionData[] allVisions = { normalVision, nightScopeVision, invertedVision, thermalVision };

        foreach (var data in allVisions)
        {
            if (data != null)
            {
                data.currentBattery = data.maxBattery;
            }
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        HandleInput();
        HandleBatteryDrain();
    }

    void HandleInput()
    {
        if (cooldownTimer > 0f) return;

        // キー入力による切り替え
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleVision(VisionType.NightScope);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleVision(VisionType.Inverted);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleVision(VisionType.Thermal);
        }
    }

    public void ToggleVision(VisionType vision)
    {
        // 同じキーで戻す
        if (currentVision == vision)
        {
            SetVision(VisionType.Normal);
        }
        else
        {
            // バッテリーが残ってる場合だけ切り替え
            VisionData data = GetVisionData(vision);
            if (data != null && !data.IsDepleted)
            {
                SetVision(vision);
            }
            else
            {
                Debug.Log($"⚠ {vision} はバッテリー切れ！");
            }
        }

        cooldownTimer = visionCooldown;
    }

    void SetVision(VisionType newVision)
    {
        currentVision = newVision;
        Debug.Log($"▶ 現在の視界: {currentVision}");
    }

    void HandleBatteryDrain()
    {
        // 通常視界では消費しない
        if (currentVision == VisionType.Normal) return;

        VisionData data = GetVisionData(currentVision);
        if (data == null) return;

        // バッテリーを減らす
        data.currentBattery -= data.drainRate * Time.deltaTime;

        // 下限チェック
        if (data.currentBattery <= 0f)
        {
            data.currentBattery = 0f;
            Debug.Log($"🔋 {currentVision} のバッテリーが切れました！");
            SetVision(VisionType.Normal);
        }
    }

    VisionData GetVisionData(VisionType type)
    {
        return type switch
        {
            VisionType.Normal => normalVision,
            VisionType.NightScope => nightScopeVision,
            VisionType.Inverted => invertedVision,
            VisionType.Thermal => thermalVision,
            _ => null
        };
    }
}
