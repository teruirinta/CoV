using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VisionEffectController : MonoBehaviour
{
    [Header("各視界用のVolume")]
    public Volume normalVolume;
    public Volume nightScopeVolume;
    public Volume invertVolume;
    public Volume thermalVolume;

    [Header("メガネのモデル")]
    public GameObject nightScopeModel;
    public GameObject invertModel;
    public GameObject thermalModel;

    [Header("共通設定")]
    [Tooltip("親オブジェクトにあるAnimator")]
    public Animator glassesAnimator;
    public float wearAnimationTime = 0.5f;

    private bool defaultFogState;
    private VisionType currentVision = VisionType.Normal;
    private Coroutine visionRoutine;

    void Start()
    {
        defaultFogState = RenderSettings.fog;
        SetAllVolumesOff();
        HideAllGlasses();

        // ★起動時は「何もしない」状態にする
        PlayIdleAnimation();
    }

    public void ChangeVision(VisionType newVision)
    {
        if (visionRoutine != null) StopCoroutine(visionRoutine);
        visionRoutine = StartCoroutine(VisionSequence(newVision));
    }

    IEnumerator VisionSequence(VisionType newVision)
    {
        // ① すでにメガネをかけている場合 → 外す
        if (currentVision != VisionType.Normal)
        {
            DisableVision(currentVision);
            PlayRemoveAnimation(); // 外す動き

            yield return new WaitForSeconds(wearAnimationTime);

            // 外し終わったらモデルを非表示
            GameObject oldGlasses = GetGlassesModel(currentVision);
            if (oldGlasses != null) oldGlasses.SetActive(false);
        }

        // ② Normal（裸眼）に戻る場合
        if (newVision == VisionType.Normal)
        {
            currentVision = VisionType.Normal;

            // ★ここが重要：外し終わったら「何もしない(Empty)」状態へ移行
            PlayIdleAnimation();

            yield break;
        }

        // ③ 新しいメガネをかける
        GameObject nextGlasses = GetGlassesModel(newVision);
        if (nextGlasses != null) nextGlasses.SetActive(true);

        PlayWearAnimation(newVision);

        yield return new WaitForSeconds(wearAnimationTime);

        // ④ 視界エフェクトON
        EnableVision(newVision);
        currentVision = newVision;
    }

    // ... (GetGlassesModel, HideAllGlasses, SetAllVolumesOff などは変更なし) ...
    GameObject GetGlassesModel(VisionType type)
    {
        switch (type)
        {
            case VisionType.NightScope: return nightScopeModel;
            case VisionType.Inverted: return invertModel;
            case VisionType.MemoryVision: return thermalModel;
            default: return null;
        }
    }

    void HideAllGlasses()
    {
        if (nightScopeModel) nightScopeModel.SetActive(false);
        if (invertModel) invertModel.SetActive(false);
        if (thermalModel) thermalModel.SetActive(false);
    }

    #region Animation

    // ★追加：何もしない状態へ遷移させる関数
    void PlayIdleAnimation()
    {
        // Animatorで作った空のステート名（"Empty" や "Idle"）を指定してください
        glassesAnimator.Play("Empty");
    }

    void PlayWearAnimation(VisionType vision)
    {
        glassesAnimator.SetInteger("VisionType", (int)vision);
        glassesAnimator.SetFloat("Speed", 1f);
        glassesAnimator.Play("A Animation", 0, 0f);
    }

    void PlayRemoveAnimation()
    {
        glassesAnimator.SetFloat("Speed", -1f);
        glassesAnimator.Play("A Animation", 0, 1f);
    }

    #endregion

    #region Vision Control
    // ... (EnableVision, DisableVision, SetAllVolumesOff, SetFog はそのまま) ...

    void EnableVision(VisionType vision)
    {
        SetAllVolumesOff();
        switch (vision)
        {
            case VisionType.NightScope:
                if (nightScopeVolume) nightScopeVolume.enabled = true;
                SetFog(false);
                break;
            case VisionType.Inverted:
                if (invertVolume) invertVolume.enabled = true;
                SetFog(defaultFogState);
                break;
            case VisionType.MemoryVision:
                if (thermalVolume) thermalVolume.enabled = true;
                SetFog(defaultFogState);
                break;
        }
    }

    void DisableVision(VisionType vision)
    {
        SetAllVolumesOff();
        SetFog(defaultFogState);
    }

    void SetAllVolumesOff()
    {
        if (normalVolume) normalVolume.enabled = false;
        if (nightScopeVolume) nightScopeVolume.enabled = false;
        if (invertVolume) invertVolume.enabled = false;
        if (thermalVolume) thermalVolume.enabled = false;
    }

    void SetFog(bool enabled)
    {
        if (RenderSettings.fog != enabled) RenderSettings.fog = enabled;
    }
    #endregion
}