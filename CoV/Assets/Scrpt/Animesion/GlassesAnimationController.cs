using UnityEngine;
using System.Collections;

public class GlassesAnimationController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Override Controllers")]
    public AnimatorOverrideController glassAOverride;
    public AnimatorOverrideController glassBOverride;
    public AnimatorOverrideController glassCOverride;

    [Header("Vision")]
    public VisionType visionA = VisionType.NightScope;
    public VisionType visionB = VisionType.Inverted;
    public VisionType visionC = VisionType.MemoryVision;

    VisionType currentVision = VisionType.Normal;
    bool isPlaying = false;

    void Update()
    {
        if (isPlaying) return;
        if (VisionManager.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Toggle(glassAOverride, visionA);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Toggle(glassBOverride, visionB);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            Toggle(glassCOverride, visionC);
    }

    // =============================
    // 切り替え制御
    // =============================
    void Toggle(AnimatorOverrideController nextOverride, VisionType nextVision)
    {
        if (isPlaying) return;

        if (currentVision == VisionType.Normal)
        {
            StartCoroutine(WearRoutine(nextOverride, nextVision));
        }
        else if (currentVision == nextVision)
        {
            StartCoroutine(RemoveRoutine());
        }
        else
        {
            StartCoroutine(SwapRoutine(nextOverride, nextVision));
        }
    }

    // =============================
    // 装着
    // =============================
    IEnumerator WearRoutine(AnimatorOverrideController overrideController, VisionType vision)
    {
        isPlaying = true;

        animator.runtimeAnimatorController = overrideController;
        animator.ResetTrigger("Remove");
        animator.SetTrigger("Wear");

        yield return WaitForAnimationEnd("Wear");

        if (VisionManager.Instance != null)
        {
            VisionManager.Instance.SetVision(vision);
        }

        currentVision = vision;
        isPlaying = false;
    }

    // =============================
    // 取り外し
    // =============================
    IEnumerator RemoveRoutine()
    {
        isPlaying = true;

        animator.ResetTrigger("Wear");
        animator.SetTrigger("Remove");

        yield return WaitForAnimationEnd("Remove");

        if (VisionManager.Instance != null)
        {
            VisionManager.Instance.SetVision(VisionType.Normal);
        }

        currentVision = VisionType.Normal;
        isPlaying = false;
    }

    // =============================
    // 切り替え（外す → 付ける）
    // =============================
    IEnumerator SwapRoutine(AnimatorOverrideController nextOverride, VisionType nextVision)
    {
        isPlaying = true;

        animator.ResetTrigger("Wear");
        animator.SetTrigger("Remove");
        yield return WaitForAnimationEnd("Remove");

        animator.runtimeAnimatorController = nextOverride;
        animator.ResetTrigger("Remove");
        animator.SetTrigger("Wear");
        yield return WaitForAnimationEnd("Wear");

        if (VisionManager.Instance != null)
        {
            VisionManager.Instance.SetVision(nextVision);
        }

        currentVision = nextVision;
        isPlaying = false;
    }

    // =============================
    // アニメーション終了待ち（安全版）
    // =============================
    IEnumerator WaitForAnimationEnd(string stateName)
    {
        // 遷移完了待ち
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        // 再生完了待ち
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
    }
}