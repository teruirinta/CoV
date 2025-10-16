using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照")]
    public Image gaugeImage; // 円形ゲージ
    public Image iconImage;  // アイコン画像

    [Header("設定")]
    public VisionType targetVision; // 対応する視界タイプ

    private VisionManager visionManager;

    void Start()
    {
        visionManager = VisionManager.Instance;
    }

    void Update()
    {
        if (visionManager == null) return;

        // 対応する視界のデータ取得
        var vision = visionManager.GetVisionData(targetVision);
        if (vision == null) return;

        // 現在の残量をゲージに反映
        float fillAmount = vision.currentBattery / vision.maxBattery;
        gaugeImage.fillAmount = Mathf.Clamp01(fillAmount);

        // 視界ごとに色設定
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
