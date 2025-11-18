using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照")]
    public Image gaugeImage; // 円形ゲージ
    public Image iconImage;  // アイコン画像
    public Image batteryCD;  // ★追加：クールダウン表示用UI

    [Header("このUIが担当する視界タイプ")]
    public VisionType targetVision; // NightScope / Inverted / Thermal

    private VisionManager visionManager;

    void Start()
    {
        visionManager = VisionManager.Instance;

        // 起動時は非表示
        if (batteryCD != null)
            batteryCD.enabled = false;
    }

    void Update()
    {
        if (visionManager == null) return;

        // このUIが担当する視界データを取得
        var visionData = visionManager.GetVisionData(targetVision);
        if (visionData == null) return;

        // バッテリー残量（0～1に正規化）
        float fillAmount = visionData.currentBattery / visionData.maxBattery;
        gaugeImage.fillAmount = Mathf.Clamp01(fillAmount);

        // ゲージの色
        switch (targetVision)
        {
            case VisionType.NightScope:
                gaugeImage.color = Color.green;
                break;
            case VisionType.Inverted:
                gaugeImage.color = Color.cyan;
                break;
            case VisionType.Thermal:
                gaugeImage.color = Color.red;
                break;
            default:
                gaugeImage.color = Color.gray;
                break;
        }

        // ★ クールダウン中なら BatteryCD を表示
        bool isCooldown = visionManager.CooldownTimer > 0f;

        if (batteryCD != null)
            batteryCD.enabled = isCooldown;
    }
}
