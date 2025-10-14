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
        // シングルトン的にアクセス可能にする
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // クールダウンの経過処理
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        // 🎮 Xboxコントローラー対応
        if (Input.GetButtonDown("Fire3")) // Bボタン
        {
            ToggleVision(VisionType.NightScope);
        }
        else if (Input.GetButtonDown("Fire1")) // Xボタン
        {
            ToggleVision(VisionType.Inverted);
        }
        else if (Input.GetButtonDown("Fire2")) // Yボタン
        {
            ToggleVision(VisionType.Thermal);
        }

        // ⌨️ キーボード対応（1,2,3キー）
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
        // 同じキーを押したら通常視界に戻す
        if (CurrentVision == vision)
            CurrentVision = VisionType.Normal;
        else
            CurrentVision = vision;

        // クールダウン再設定
        cooldownTimer = visionCooldown;

        Debug.Log($"▶ 現在の視界: {CurrentVision}");
    }
}
