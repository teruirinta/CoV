using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照")]
    public Image gaugeImage; // 円形ゲージ
    public Image iconImage;  // アイコン画像
    public Image batteryCD;  // ★円形クールダウンUI（Filled Image）

    [Header("このUIが担当する視界タイプ")]
    public VisionType targetVision; // NightScope / Inverted / Thermal

    private VisionManager visionManager;

    void Start()
    {
        visionManager = VisionManager.Instance;

        // 起動時は非表示
        if (batteryCD != null)
        {
            batteryCD.enabled = false;
            batteryCD.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (visionManager == null) return;

        // ◆ このUIが担当する視界データを取得
        var visionData = visionManager.GetVisionData(targetVision);
        if (visionData == null) return;

        // -------------------------
        // ◆ バッテリーゲージ更新
        // -------------------------
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
            case VisionType.MemoryVision:
                gaugeImage.color = Color.red;
                break;
            default:
                gaugeImage.color = Color.gray;
                break;
        }

        // -------------------------
        // ◆ クールダウン円形ゲージ
        // -------------------------
        float cd = visionManager.CooldownTimer;
        float cdMax = visionManager.CooldownDuration;

        if (batteryCD != null)
        {
            if (cd > 0f)
            {
                // 表示
                batteryCD.enabled = true;

                // 1 → 0 に減る
                float ratio = Mathf.Clamp01(cd / cdMax);
                batteryCD.fillAmount = ratio;
            }
            else
            {
                // クールダウン終了 → 非表示
                batteryCD.enabled = false;
                batteryCD.fillAmount = 0f;
            }
        }
    }
}
