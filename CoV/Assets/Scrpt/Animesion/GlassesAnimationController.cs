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

    [Header("透明化対象 Renderer")]
    public Renderer[] glassesRenderers;
    public Renderer[] playerRenderers;

    [Header("透明度設定")]
    [Range(0f, 1f)]
    public float invisibleAlpha = 0.2f;

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
            StartCoroutine(WearRoutine(nextOverride, nextVision));
        else if (currentVision == nextVision)
            StartCoroutine(RemoveRoutine());
        else
            StartCoroutine(SwapRoutine(nextOverride, nextVision));
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

        VisionManager.Instance.SetVision(vision);
        currentVision = vision;

        // ★ アニメ終了後に透明化
        SetInvisible(true);

        isPlaying = false;
    }

    // =============================
    // 取り外し
    // =============================
    IEnumerator RemoveRoutine()
    {
        isPlaying = true;

        // ★ 先に見た目を戻す
        SetInvisible(false);

        animator.ResetTrigger("Wear");
        animator.SetTrigger("Remove");

        yield return WaitForAnimationEnd("Remove");

        VisionManager.Instance.SetVision(VisionType.Normal);
        currentVision = VisionType.Normal;

        isPlaying = false;
    }

    // =============================
    // 切り替え（外す → 付ける）
    // =============================
    IEnumerator SwapRoutine(AnimatorOverrideController nextOverride, VisionType nextVision)
    {
        isPlaying = true;

        // 外す
        SetInvisible(false);
        animator.SetTrigger("Remove");
        yield return WaitForAnimationEnd("Remove");

        // 付ける
        animator.runtimeAnimatorController = nextOverride;
        animator.SetTrigger("Wear");
        yield return WaitForAnimationEnd("Wear");

        VisionManager.Instance.SetVision(nextVision);
        currentVision = nextVision;

        // ★ 透明化
        SetInvisible(true);

        isPlaying = false;
    }

    // =============================
    // アニメーション終了待ち
    // =============================
    IEnumerator WaitForAnimationEnd(string stateName)
    {
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }

    // =============================
    // 透明化制御
    // =============================
    void SetInvisible(bool invisible)
    {
        float alpha = invisible ? invisibleAlpha : 1f;

        SetAlpha(glassesRenderers, alpha);
        SetAlpha(playerRenderers, alpha);
    }

    void SetAlpha(Renderer[] renderers, float alpha)
    {
        if (renderers == null) return;

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }
}