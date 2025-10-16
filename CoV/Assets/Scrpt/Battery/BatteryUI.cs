using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照")]
    public Image gaugeImage; // 円形ゲージ
    public Image iconImage;  // アイコン画像

    [Header("このUIが担当する視界タイプ")]
    public VisionType targetVision; // NightScope / Inverted / Thermal

    private VisionManager visionManager;

    void Start()
    {
        visionManager = VisionManager.Instance;
    }

    void Update()
    {
        if (visionManager == null) return;

        // このUIが担当する視界データを取得
        var vision = visionManager.GetVisionData(targetVision);
        if (vision == null) return;

        // バッテリー残量（0～1に正規化）
        float fillAmount = vision.currentBattery / vision.maxBattery;
        gaugeImage.fillAmount = Mathf.Clamp01(fillAmount);

        // ゲージの色設定（任意）
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
    }
}
