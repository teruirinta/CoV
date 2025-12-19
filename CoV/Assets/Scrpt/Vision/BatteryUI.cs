using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照 (子要素 de パーツ)")]
    public Image gaugeImage; // 円形ゲージ
    public Image iconImage;
    public Image batteryCD;

    [Header("このUI(親オブジェクト)が担当する視界タイプ")]
    public VisionType targetVision;

    [Header("スライド位置設定 (親の座標)")]
    public Vector2 visibleAnchoredPos; // 表示時の座標
    public Vector2 hiddenAnchoredPos;  // 隠し時の座標
    public float lerpSpeed = 10f;      // 移動スピード

    private VisionManager visionManager;
    private RectTransform rectTransform; // 親自身のRectTransform

    void Start()
    {
        visionManager = VisionManager.Instance;
        rectTransform = GetComponent<RectTransform>();

        // 開始時は隠し位置にセット
        rectTransform.anchoredPosition = hiddenAnchoredPos;

        if (batteryCD != null)
        {
            batteryCD.enabled = false;
            batteryCD.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (visionManager == null) return;

        // --- 1. 親オブジェクトのスライド処理 ---
        bool isCurrentMode = (visionManager.CurrentVision == targetVision);
        Vector2 targetPos = isCurrentMode ? visibleAnchoredPos : hiddenAnchoredPos;

        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.deltaTime * lerpSpeed);

        float cd = visionManager.CooldownTimer;

        if (!isCurrentMode &&
            Vector2.Distance(rectTransform.anchoredPosition, hiddenAnchoredPos) < 0.1f &&
            cd <= 0f)
        {
            return;
        }

        // --- 2. バッテリーデータの更新 ---
        var visionData = visionManager.GetVisionData(targetVision);
        if (visionData == null) return;

        float rawRatio = visionData.currentBattery / visionData.maxBattery;
        float steppedFill;

        // 段階的な表示ロジック
        if (rawRatio >= 0.99f) steppedFill = 1.0f;
        else if (rawRatio > 0.75f) steppedFill = 1.0f;
        else if (rawRatio > 0.50f) steppedFill = 0.75f;
        else if (rawRatio > 0.25f) steppedFill = 0.50f;
        else if (rawRatio > 0f) steppedFill = 0.25f;
        else steppedFill = 0f;

        // --- 3. ゲージの色と残量の反映 ---
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = steppedFill;

            // ★追加：残量に応じた色の切り替え
            if (rawRatio <= 0.25f)
            {
                gaugeImage.color = Color.red;    // 25%以下
            }
            else if (rawRatio <= 0.50f)
            {
                gaugeImage.color = Color.yellow; // 50%以下
            }
            else
            {
                gaugeImage.color = Color.green;  // 通常時
            }
        }

        // クールダウン表示の更新
        UpdateCooldown(cd);
    }

    void UpdateCooldown(float cd)
    {
        if (batteryCD == null) return;

        float cdMax = visionManager.CooldownDuration;

        if (cd > 0f)
        {
            batteryCD.enabled = true;
            batteryCD.fillAmount = Mathf.Clamp01(cd / cdMax);
        }
        else
        {
            batteryCD.enabled = false;
            batteryCD.fillAmount = 0f;
        }
    }
}