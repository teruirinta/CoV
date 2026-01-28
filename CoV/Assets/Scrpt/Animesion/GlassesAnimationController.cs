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

    [Header("非表示にするオブジェクト（メガネ3つ）")]
    public GameObject[] glassesRoots;
    public GameObject playerModelRoot;

    [Header("消えるまでの余韻")]
    public float hideDelayAfterAnimation = 0.3f;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip hideSE;
    public float hideSEDelay = 0.1f;

    VisionType currentVision = VisionType.Normal;
    bool isPlaying = false;

    void Start()
    {
        SetVisible(false);
    }

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
    // 装着（Wear）
    // =============================
    IEnumerator WearRoutine(AnimatorOverrideController overrideController, VisionType vision)
    {
        isPlaying = true;

        SetVisible(true);

        animator.runtimeAnimatorController = overrideController;
        animator.Play("Wear", 0, 0f);

        yield return WaitForStateEnd("Wear");

        VisionManager.Instance.SetVision(vision);
        currentVision = vision;

        yield return HideWithSE();

        isPlaying = false;
    }

    // =============================
    // 取り外し（Remove）
    // =============================
    IEnumerator RemoveRoutine()
    {
        isPlaying = true;

        SetVisible(true);

        animator.Play("Remove", 0, 0f);

        yield return WaitForStateEnd("Remove");

        VisionManager.Instance.SetVision(VisionType.Normal);
        currentVision = VisionType.Normal;

        yield return HideWithSE();

        isPlaying = false;
    }

    // =============================
    // 切り替え（Remove → Wear）
    // =============================
    IEnumerator SwapRoutine(AnimatorOverrideController nextOverride, VisionType nextVision)
    {
        isPlaying = true;

        SetVisible(true);

        // 外す
        animator.Play("Remove", 0, 0f);
        yield return WaitForStateEnd("Remove");

        // 付ける
        animator.runtimeAnimatorController = nextOverride;
        animator.Play("Wear", 0, 0f);
        yield return WaitForStateEnd("Wear");

        VisionManager.Instance.SetVision(nextVision);
        currentVision = nextVision;

        yield return HideWithSE();

        isPlaying = false;
    }

    // =============================
    // State 再生完了待ち
    // =============================
    IEnumerator WaitForStateEnd(string stateName)
    {
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }

    // =============================
    // 非表示 + SE
    // =============================
    IEnumerator HideWithSE()
    {
        yield return new WaitForSeconds(hideDelayAfterAnimation);

        SetVisible(false);

        yield return new WaitForSeconds(hideSEDelay);

        if (audioSource && hideSE)
            audioSource.PlayOneShot(hideSE);
    }

    // =============================
    // 表示切り替え
    // =============================
    void SetVisible(bool visible)
    {
        if (glassesRoots != null)
        {
            foreach (var g in glassesRoots)
            {
                if (g) g.SetActive(visible);
            }
        }

        if (playerModelRoot)
            playerModelRoot.SetActive(visible);
    }
}