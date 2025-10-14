using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisionEffectController : MonoBehaviour
{
    [Header("�e���E�p��Volume")]
    [Tooltip("�ʏ펋�E�p�i�W����PostProcess Volume�j")]
    public Volume normalVolume;

    [Tooltip("�i�C�g�X�R�[�v�i�Î��j�pVolume")]
    public Volume nightScopeVolume;

    [Tooltip("�㉺���]���E�pVolume�i���]���o�����G�t�F�N�g�p�j")]
    public Volume invertVolume;

    [Tooltip("�T�[���O���t�B���E�pVolume")]
    public Volume thermalVolume;

    void Start()
    {
        // ������Ԃ�Normal�ɐݒ�
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
