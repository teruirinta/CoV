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

    private bool defaultFogState; // 起動時のフォグ設定を保存

    void Start()
    {
        // 起動時のフォグ設定を記憶
        defaultFogState = RenderSettings.fog;
    }

    void Update()
    {
        if (VisionManager.Instance == null) return;

        switch (VisionManager.Instance.CurrentVision)
        {
            case VisionType.Normal:
                SetActiveVolume(normalVolume);
                SetFog(defaultFogState); // 通常は元の設定
                break;

            case VisionType.NightScope:
                SetActiveVolume(nightScopeVolume);
                SetFog(false); // 🌙 ナイトスコープ時はフォグを無効化
                break;

            case VisionType.Inverted:
                SetActiveVolume(invertVolume);
                SetFog(defaultFogState); // 他は通常通り
                break;

            case VisionType.Thermal:
                SetActiveVolume(thermalVolume);
                SetFog(defaultFogState);
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

    void SetFog(bool enabled)
    {
        if (RenderSettings.fog != enabled)
        {
            RenderSettings.fog = enabled;
            Debug.Log($"🌫️ Fog状態変更: {(enabled ? "有効" : "無効")}");
        }
    }
}
