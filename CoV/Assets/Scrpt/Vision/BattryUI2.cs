using UnityEngine;
using UnityEngine.UI;

public class BattryUI2 : MonoBehaviour
{
    [Header("UI参照")]
    public Image gaugeImage; // バッテリー本体ゲージ（円形でも棒でも可）
    public Image iconImage;  // アイコン画像
    public Image batteryCD;  // ★縦方向クールダウンUI（Filled Image / Vertical）

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

            // ★ 必須設定（保険）
            batteryCD.type = Image.Type.Filled;
            batteryCD.fillMethod = Image.FillMethod.Vertical;
            batteryCD.fillOrigin = 0; // Top（上から減っていく）
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

        // ゲージ色
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
        // ◆ クールダウン縦ゲージ
        // -------------------------
        float cd = visionManager.CooldownTimer;
        float cdMax = visionManager.CooldownDuration;

        if (batteryCD != null)
        {
            if (cd > 0f)
            {
                batteryCD.enabled = true;

                // cd(3→0) → 1→0 へスケール
                float ratio = Mathf.Clamp01(cd / cdMax);

                // ★ 上から減る縦ゲージ
                batteryCD.fillAmount = ratio;
            }
            else
            {
                batteryCD.enabled = false;
                batteryCD.fillAmount = 0f;
            }
        }
    }
}
