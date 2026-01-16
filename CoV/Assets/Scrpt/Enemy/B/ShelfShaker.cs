using UnityEngine;

public class ShelfShaker : MonoBehaviour
{
    [Header("震える対象")]
    public Transform targetObject;

    [Header("ガタガタ設定")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 10f;
    public float minShakeInterval = 2f;
    public float maxShakeInterval = 5f;
    public float shakeDuration = 1f;

    [Header("ガタガタ音")]
    public AudioSource shakeAudio;

    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float shakeTimeRemaining = 0f;
    private bool isBroken = false; // ← 壊れたかどうかのフラグ

    void Start()
    {
        if (targetObject != null)
        {
            originalPos = targetObject.localPosition;
        }

        ResetShakeTimer();
    }

    void Update()
    {
        if (targetObject == null || isBroken) return; // ← 壊れてたら何もしない！

        if (shakeTimeRemaining > 0f)
        {
            Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f,
                0f
            ) * shakeAmount;

            targetObject.localPosition = originalPos + shakeOffset;
            shakeTimeRemaining -= Time.deltaTime;

            if (shakeAudio != null && !shakeAudio.isPlaying)
            {
                shakeAudio.loop = true;
                shakeAudio.Play();
            }
        }
        else
        {
            targetObject.localPosition = originalPos;
            shakeTimer -= Time.deltaTime;

            if (shakeAudio != null && shakeAudio.isPlaying)
            {
                shakeAudio.Stop();
            }

            if (shakeTimer <= 0f)
            {
                shakeTimeRemaining = shakeDuration;
                ResetShakeTimer();
            }
        }
    }

    void ResetShakeTimer()
    {
        shakeTimer = Random.Range(minShakeInterval, maxShakeInterval);
    }

    // 外部から呼び出して壊す処理
    public void BreakShelf()
    {
        isBroken = true;

        if (shakeAudio != null && shakeAudio.isPlaying)
        {
            shakeAudio.Stop();
        }

        // ここに壊れるアニメーションやエフェクトを追加してもOK！
    }
}
