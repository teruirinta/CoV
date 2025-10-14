using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisionEffectController : MonoBehaviour
{
    [Header("各視界用のVolume")]
    [Tooltip("通常視界用（標準のPostProcess Volume）")]
    public Volume normalVolume;

    [Tooltip("ナイトスコープ（暗視）用Volume")]
    public Volume nightScopeVolume;

    [Tooltip("上下反転視界用Volume（反転演出や特殊エフェクト用）")]
    public Volume invertVolume;

    [Tooltip("サーモグラフィ視界用Volume")]
    public Volume thermalVolume;

    void Start()
    {
        // 初期状態をNormalに設定
        SetActiveVolume(normalVolume);
    }

    void Update()
    {
        if (VisionManager.Instance == null) return;

        switch (VisionManager.Instance.CurrentVision)
        {
            case VisionType.Normal:
                SetActiveVolume(normalVolume);
                break;
            case VisionType.NightScope:
                SetActiveVolume(nightScopeVolume);
                break;
            case VisionType.Inverted:
                SetActiveVolume(invertVolume);
                break;
            case VisionType.Thermal:
                SetActiveVolume(thermalVolume);
                break;
        }
    }

    void SetActiveVolume(Volume active)
    {
        if (normalVolume) normalVolume.enabled = (active == normalVolume);
        if (nightScopeVolume) nightScopeVolume.enabled = (active == nightScopeVolume);
        if (invertVolume) invertVolume.enabled = (active == invertVolume);
        if (thermalVolume) thermalVolume.enabled = (active == thermalVolume);
    }
}
