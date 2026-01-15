using UnityEngine;
using System.Collections;

public class GlassesAnimationController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Glass Animation State")]
    public string glassA = "Glass_A";
    public string glassB = "Glass_B";
    public string glassC = "Glass_C";

    [Header("Vision")]
    public VisionType visionA;
    public VisionType visionB;
    public VisionType visionC;

    string currentGlass = "";
    VisionType pendingVision = VisionType.Normal;
    bool isPlaying = false;

    void Update()
    {
        if (isPlaying || VisionManager.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Toggle(glassA, visionA);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Toggle(glassB, visionB);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            Toggle(glassC, visionC);
    }

    // =============================
    // 切り替え
    // =============================
    void Toggle(string nextGlass, VisionType nextVision)
    {
        if (isPlaying) return;

        if (string.IsNullOrEmpty(currentGlass))
        {
            StartCoroutine(WearRoutine(nextGlass, nextVision));
        }
        else if (currentGlass == nextGlass)
        {
            StartCoroutine(RemoveRoutine());
        }
        else
        {
            StartCoroutine(SwapRoutine(nextGlass, nextVision));
        }
    }

    // =============================
    // 装着
    // =============================
    IEnumerator WearRoutine(string glassName, VisionType vision)
    {
        isPlaying = true;
        pendingVision = vision;

        Debug.Log($"👓 {glassName} 装着開始");

        animator.SetFloat("Speed", 1f);
        animator.Play(glassName, 0, 0f);

        yield return new WaitForSeconds(GetClipLength(glassName));

        VisionManager.Instance.SetVision(pendingVision);
        currentGlass = glassName;

        Debug.Log($"✅ {glassName} 装着完了");

        isPlaying = false;
    }

    // =============================
    // 取り外し
    // =============================
    IEnumerator RemoveRoutine()
    {
        isPlaying = true;

        Debug.Log($"🧤 {currentGlass} 取り外し開始");

        animator.SetFloat("Speed", -1f);
        animator.Play(currentGlass, 0, 0.999f);

        yield return new WaitForSeconds(GetClipLength(currentGlass));

        VisionManager.Instance.SetVision(VisionType.Normal);
        currentGlass = "";

        Debug.Log("✅ メガネ取り外し完了");

        isPlaying = false;
    }

    // =============================
    // 切り替え
    // =============================
    IEnumerator SwapRoutine(string nextGlass, VisionType nextVision)
    {
        yield return StartCoroutine(RemoveRoutine());
        yield return StartCoroutine(WearRoutine(nextGlass, nextVision));
    }

    // =============================
    // クリップ長取得
    // =============================
    float GetClipLength(string stateName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
            {
                Debug.Log($"🎞 {clip.name} / {clip.length} 秒");
                return clip.length;
            }
        }

        Debug.LogWarning($"⚠ アニメーションクリップ未検出: {stateName}");
        return 0f;
    }
}