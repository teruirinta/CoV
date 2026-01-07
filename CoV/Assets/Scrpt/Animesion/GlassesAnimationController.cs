using UnityEngine;
using System.Collections;

/// <summary>
/// メガネ装着アニメーション制御
/// ・1 / 2 / 3 キーで A / B / C
/// ・同じキーで外す
/// ・別のキーなら「外す → かける」
/// ・外す動きは逆再生
/// ・アニメ完了後に視界を変更
/// </summary>
public class GlassesAnimationController : MonoBehaviour
{
    [Header("メガネ Animator")]
    public Animator glassesAnimator;

    [Header("アニメーション名（Animator State名）")]
    public string glassAAnimation = "Glass_A";
    public string glassBAnimation = "Glass_B";
    public string glassCAnimation = "Glass_C";

    [Header("アニメーション時間（秒）")]
    public float animationTime = 0.5f;

    private Coroutine currentRoutine;

    // 現在かけているメガネ（"" = なし）
    private string currentAnimation = "";

    // 現在の視界
    private VisionType currentVision = VisionType.Normal;

    void Update()
    {
        if (VisionManager.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleGlasses(glassAAnimation, VisionType.NightScope);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleGlasses(glassBAnimation, VisionType.Inverted);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ToggleGlasses(glassCAnimation, VisionType.MemoryVision);
    }

    // =============================
    // メガネ切り替え（トグル）
    // =============================
    void ToggleGlasses(string animationName, VisionType nextVision)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        // 同じメガネ → 外す
        if (currentAnimation == animationName)
        {
            currentRoutine = StartCoroutine(RemoveRoutine());
        }
        // 何もかけていない → かける
        else if (string.IsNullOrEmpty(currentAnimation))
        {
            currentRoutine = StartCoroutine(WearRoutine(animationName, nextVision));
        }
        // 別のメガネ → 外してからかける
        else
        {
            currentRoutine = StartCoroutine(SwapRoutine(animationName, nextVision));
        }
    }

    // =============================
    // メガネをかける
    // =============================
    IEnumerator WearRoutine(string animationName, VisionType nextVision)
    {
        currentAnimation = animationName;

        // 正再生
        glassesAnimator.SetFloat("Speed", 1f);
        glassesAnimator.Play(animationName, 0, 0f);

        yield return new WaitForSeconds(animationTime);

        // 視界変更
        currentVision = nextVision;
        VisionManager.Instance.SetVision(nextVision);
    }

    // =============================
    // メガネを外す（逆再生）
    // =============================
    IEnumerator RemoveRoutine()
    {
        if (string.IsNullOrEmpty(currentAnimation))
            yield break;

        // 視界を通常に戻す
        currentVision = VisionType.Normal;
        VisionManager.Instance.SetVision(VisionType.Normal);

        // 逆再生
        glassesAnimator.SetFloat("Speed", -1f);
        glassesAnimator.Play(currentAnimation, 0, 1f);

        yield return new WaitForSeconds(animationTime);

        currentAnimation = "";
    }

    // =============================
    // メガネを入れ替える
    // =============================
    IEnumerator SwapRoutine(string nextAnimation, VisionType nextVision)
    {
        yield return StartCoroutine(RemoveRoutine());
        yield return StartCoroutine(WearRoutine(nextAnimation, nextVision));
    }
}