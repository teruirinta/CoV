using UnityEngine;

public class ShelfShaker : MonoBehaviour
{
    [Header("k‚¦‚é‘ÎÛ")]
    public Transform targetObject;

    [Header("ƒKƒ^ƒKƒ^Ý’è")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 10f;
    public float minShakeInterval = 2f;
    public float maxShakeInterval = 5f;
    public float shakeDuration = 1f;

    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float shakeTimeRemaining = 0f;

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
        if (targetObject == null) return;

        if (shakeTimeRemaining > 0f)
        {
            Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f,
                0f
            ) * shakeAmount;

            targetObject.localPosition = originalPos + shakeOffset;
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            targetObject.localPosition = originalPos;
            shakeTimer -= Time.deltaTime;

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
}
