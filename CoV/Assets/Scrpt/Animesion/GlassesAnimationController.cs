using UnityEngine;
using System.Collections;

public class GlassesAnimationController : MonoBehaviour
{
    [Header("メガネAnimator")]
    public Animator glassesAnimator;

    [Header("アニメーション時間（秒）")]
    public float animationTime = 0.5f;

    private Coroutine currentRoutine;
    private string currentAnimation = "";

    // =============================
    // 外部から呼ぶ用
    // =============================

    // メガネをかける（A / B / C）
    public void WearGlasses(string animationName)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(WearRoutine(animationName));
    }

    // メガネを外す
    public void RemoveGlasses()
    {
        if (string.IsNullOrEmpty(currentAnimation)) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(RemoveRoutine());
    }

    // =============================
    // Coroutine
    // =============================

    IEnumerator WearRoutine(string animationName)
    {
        currentAnimation = animationName;

        glassesAnimator.SetFloat("Speed", 1f);
        glassesAnimator.Play(animationName, 0, 0f);

        yield return new WaitForSeconds(animationTime);
    }

    IEnumerator RemoveRoutine()
    {
        glassesAnimator.SetFloat("Speed", -1f);
        glassesAnimator.Play(currentAnimation, 0, 1f);

        yield return new WaitForSeconds(animationTime);

        currentAnimation = "";
    }
}