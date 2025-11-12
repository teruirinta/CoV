using UnityEngine;

public class BEnemy : MonoBehaviour
{
    [Header("視界反転設定")]
    private bool isUpsideDown = false;

    [Header("がたがた動き設定")]
    public bool enableShake = true;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;
    public float minShakeInterval = 10f;
    public float maxShakeInterval = 30f;

    [Header("プレイヤー関連")]
    public Transform player;          // プレイヤーのTransform
    public float detectionRange;
    public float chaseSpeed = 3.1f;
    public GameObject visibleObjectNear;
    public GameObject visibleObjectFar;

    private Vector3 originalLocalPosition;
    private float shakeTimer = 0f;
    private float shakeTimeRemaining = 0f;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
        ResetShakeTimer();
    }

    void Update()
    {
        if (player == null) return;

        HandleVisionInversion();
        HandleProximityVisibility();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }

        if (enableShake)
        {
            HandleShake();
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }

    void HandleVisionInversion()
    {
        if (VisionManager.Instance == null) return;

        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldBeInverted != isUpsideDown)
        {
            isUpsideDown = shouldBeInverted;

            Vector3 euler = transform.eulerAngles;
            euler.z = isUpsideDown ? 180f : 0f;
            transform.eulerAngles = euler;
        }
    }

    void HandleShake()
    {
        shakeTimer -= Time.deltaTime;

        if (shakeTimer <= 0f)
        {
            shakeTimeRemaining = shakeDuration;
            ResetShakeTimer();
        }

        if (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining -= Time.deltaTime;
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = originalLocalPosition + shakeOffset;
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }

    void HandleProximityVisibility()
    {
        if (visibleObjectNear == null || visibleObjectFar == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isNear = distance <= detectionRange;

        visibleObjectNear.SetActive(isNear);
        visibleObjectFar.SetActive(!isNear);
    }

    void ResetShakeTimer()
    {
        shakeTimer = Random.Range(minShakeInterval, maxShakeInterval);
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }
}
