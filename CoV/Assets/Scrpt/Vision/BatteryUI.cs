using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BatteryUI : MonoBehaviour
{
    [Header("UI参照 (子要素 de パーツ)")]
    public Image gaugeImage;
    public Image iconImage;
    public Image batteryCD; // クールダウン用ゲージ

    [Header("バッテリー0（空）の時の設定")]
    [Tooltip("0の時に表示し続けるUI（batteryCDと同じでもOK、別の禁止アイコン等でもOK）")]
    public Graphic emptyIndicationUI;
    [Tooltip("0の時、ゲージを何％で固定するか(1.0で満タン)")]
    [Range(0f, 1f)] public float emptyFillAmount = 1.0f;

    [Header("ピンチ時に点滅させる対象")]
    public List<Graphic> flashTargets = new List<Graphic>();
    public float flashSpeed = 8f;

    [Header("このUI(親オブジェクト)が担当する視界タイプ")]
    public VisionType targetVision;

    [Header("スライド位置設定 (親の座標)")]
    public Vector2 visibleAnchoredPos;
    public Vector2 hiddenAnchoredPos;
    public float lerpSpeed = 10f;

    private VisionManager visionManager;
    private RectTransform rectTransform;

    void Start()
    {
        visionManager = VisionManager.Instance;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = hiddenAnchoredPos;

        // 初期化
        if (batteryCD != null) { batteryCD.enabled = false; batteryCD.fillAmount = 0f; }
        if (emptyIndicationUI != null) { emptyIndicationUI.enabled = false; }
    }

    void Update()
    {
        if (visionManager == null) return;

        var visionData = visionManager.GetVisionData(targetVision);
        if (visionData == null) return;

        float rawRatio = visionData.currentBattery / visionData.maxBattery;
        float cd = visionManager.CooldownTimer;

        bool isPinch = (rawRatio <= 0.25f && rawRatio > 0f);
        bool isEmpty = (rawRatio <= 0f);

        HandleFlashing(rawRatio);

        // --- スライド処理 ---
        bool isCurrentMode = (visionManager.CurrentVision == targetVision);
        Vector2 targetPos = isCurrentMode ? visibleAnchoredPos : hiddenAnchoredPos;
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.deltaTime * lerpSpeed);

        // スリープ判定 (隠れている、CDなし、ピンチ/空でもない)
        if (!isCurrentMode &&
            Vector2.Distance(rectTransform.anchoredPosition, hiddenAnchoredPos) < 0.1f &&
            cd <= 0f && !isPinch && !isEmpty)
        {
            return;
        }

        // --- ゲージ表示 ---
        UpdateGaugeDisplay(rawRatio);

        // --- クールダウン・空状態の表示更新 ---
        UpdateStatusDisplay(cd, isEmpty);
    }

    void UpdateGaugeDisplay(float ratio)
    {
        if (gaugeImage == null) return;

        float steppedFill;
        if (ratio >= 0.99f) steppedFill = 1.0f;
        else if (ratio > 0.75f) steppedFill = 1.0f;
        else if (ratio > 0.50f) steppedFill = 0.75f;
        else if (ratio > 0.25f) steppedFill = 0.50f;
        else if (ratio > 0f) steppedFill = 0.25f;
        else steppedFill = 0f;

        gaugeImage.fillAmount = steppedFill;

        if (ratio <= 0.25f) gaugeImage.color = Color.red;
        else if (ratio <= 0.50f) gaugeImage.color = Color.yellow;
        else gaugeImage.color = Color.green;
    }

    void UpdateStatusDisplay(float cd, bool isEmpty)
    {
        // 1. バッテリーが空の時の処理
        if (isEmpty)
        {
            if (emptyIndicationUI != null)
            {
                emptyIndicationUI.enabled = true;
                // 対象がImageならFillを固定、それ以外(禁止マーク等)ならそのまま表示
                if (emptyIndicationUI is Image img) img.fillAmount = emptyFillAmount;
            }
            return; // 空の状態を最優先
        }

        // 2. クールダウン中の処理
        if (cd > 0f)
        {
            if (batteryCD != null)
            {
                batteryCD.enabled = true;
                batteryCD.fillAmount = Mathf.Clamp01(cd / visionManager.CooldownDuration);
            }
            // 空表示UIは隠す
            if (emptyIndicationUI != null && emptyIndicationUI != batteryCD) emptyIndicationUI.enabled = false;
        }
        else
        {
            // 全て正常な時
            if (batteryCD != null) batteryCD.enabled = false;
            if (emptyIndicationUI != null) emptyIndicationUI.enabled = false;
        }
    }

    void HandleFlashing(float ratio)
    {
        bool shouldFlash = (ratio <= 0.25f && ratio > 0f);
        foreach (var target in flashTargets)
        {
            if (target == null) continue;
            Color c = target.color;
            c.a = shouldFlash ? Mathf.Lerp(0.2f, 1.0f, Mathf.PingPong(Time.time * flashSpeed, 1f)) : 1.0f;
            target.color = c;
        }
    }
}