using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照 (子要素のパーツ)")]
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
        // このスクリプトがついているオブジェクト自身のRectTransformを取得
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
        // Playerの視界状態と、このUIの担当タイプを照らし合わせる
        bool isCurrentMode = (visionManager.CurrentVision == targetVision);
        Vector2 targetPos = isCurrentMode ? visibleAnchoredPos : hiddenAnchoredPos;

        // 親ごと目標座標へ移動（子がまとめてついてくる）
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.deltaTime * lerpSpeed);

        // クールタイムの現在値を取得
        float cd = visionManager.CooldownTimer;

        // --- 修正ポイント：条件の最適化 ---
        // 「非表示モード」かつ「移動がほぼ完了」かつ「クールタイムも終了(0)」の時だけ処理をスキップ
        if (!isCurrentMode &&
            Vector2.Distance(rectTransform.anchoredPosition, hiddenAnchoredPos) < 0.1f &&
            cd <= 0f)
        {
            return;
        }

        // --- 2. バッテリーデータの更新 (子要素への反映) ---
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

        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = steppedFill;
            gaugeImage.color = Color.green;
        }

        // クールダウン表示の更新
        UpdateCooldown(cd);
    }

    // 引数として現在のタイマー値を受け取って更新
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
            // タイマー終了時は確実に非表示＆0にリセット
            batteryCD.enabled = false;
            batteryCD.fillAmount = 0f;
        }
    }
}