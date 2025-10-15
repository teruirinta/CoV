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

    public VisionType CurrentVision { get; private set; } = VisionType.Normal;

    [Header("視界切り替え設定")]
    [Tooltip("視界切り替えのクールダウン時間（秒）")]
    public float visionCooldown = 3f;

    private float cooldownTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        // === 🎮 Xboxコントローラー ===
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Bボタン・1キー → ナイトスコープ
            ToggleVision(VisionType.NightScope);
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Xボタン・2キー → 上下反転
            ToggleVision(VisionType.Inverted);
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Yボタン・3キー → サーモグラフィ
            ToggleVision(VisionType.Thermal);
        }
    }

    public void ToggleVision(VisionType vision)
    {
        // 同じキーを押したら通常視界に戻す
        if (CurrentVision == vision)
            CurrentVision = VisionType.Normal;
        else
            CurrentVision = vision;

        cooldownTimer = visionCooldown;

        Debug.Log($"▶ 現在の視界: {CurrentVision}");
    }
}
