using UnityEngine;
using System.Collections;

public class GlassesAnimationController : MonoBehaviour
{
    [Header("Animator")]
    public Animator glassesAnimator;

    [Header("Animator Trigger / State –¼")]
    public string glassA = "Glass_A";
    public string glassB = "Glass_B";
    public string glassC = "Glass_C";

    [Header("‘Î‰ž‚·‚éŽ‹ŠE")]
    public VisionType visionA = VisionType.NightScope;
    public VisionType visionB = VisionType.Inverted;
    public VisionType visionC = VisionType.MemoryVision;

    private string currentGlass = "";
    private VisionType pendingVision = VisionType.Normal;
    private Coroutine currentRoutine;
    private bool isPlaying;

    void Update()
    {
        if (VisionManager.Instance == null || isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Toggle(glassA, visionA);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Toggle(glassB, visionB);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            Toggle(glassC, visionC);
    }

    void Toggle(string glassName, VisionType vision)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (currentGlass == glassName)
            currentRoutine = StartCoroutine(RemoveRoutine());
        else if (string.IsNullOrEmpty(currentGlass))
            currentRoutine = StartCoroutine(WearRoutine(glassName, vision));
        else
            currentRoutine = StartCoroutine(SwapRoutine(glassName, vision));
    }

    IEnumerator WearRoutine(string glassName, VisionType vision)
    {
        isPlaying = true;
        currentGlass = glassName;
        pendingVision = vision;

        ResetAllTriggers();

        glassesAnimator.SetFloat("Speed", 1f);
        glassesAnimator.SetTrigger(glassName);

        yield return new WaitForSeconds(GetClipLength(glassName));

        isPlaying = false;
    }

    IEnumerator RemoveRoutine()
    {
        if (string.IsNullOrEmpty(currentGlass))
            yield break;

        isPlaying = true;

        VisionManager.Instance.SetVision(VisionType.Normal);

        glassesAnimator.SetFloat("Speed", -1f);
        glassesAnimator.Play(currentGlass, 0, 0.999f);

        yield return new WaitForSeconds(GetClipLength(currentGlass));

        glassesAnimator.SetFloat("Speed", 1f);
        currentGlass = "";
        isPlaying = false;
    }

    IEnumerator SwapRoutine(string nextGlass, VisionType nextVision)
    {
        yield return StartCoroutine(RemoveRoutine());
        yield return StartCoroutine(WearRoutine(nextGlass, nextVision));
    }

    // Animation Event ‚©‚çŒÄ‚Ô
    public void OnGlassesEquipped()
    {
        VisionManager.Instance.SetVision(pendingVision);
    }

    void ResetAllTriggers()
    {
        glassesAnimator.ResetTrigger(glassA);
        glassesAnimator.ResetTrigger(glassB);
        glassesAnimator.ResetTrigger(glassC);
    }

    float GetClipLength(string clipName)
    {
        foreach (var clip in glassesAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0.5f;
    }
}