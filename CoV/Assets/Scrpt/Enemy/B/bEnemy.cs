using UnityEngine;
using UnityEngine.SceneManagement;

public class bEnemy : MonoBehaviour
{
    [Header("表示切り替え")]
    public GameObject normalObject;
    public GameObject mannequinObject;

    [Header("プレイヤー設定")]
    public Transform player;
    public float triggerDistance = 3f;
    public float chaseSpeed = 3f;

    [Header("ガタガタ動き設定")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 10f;
    public float minShakeInterval = 2f;
    public float maxShakeInterval = 5f;
    public float shakeDuration = 1f;

    private bool isTransformed = false;
    private bool isShelfUpsideDown = false;
    private Vector3 originalShelfPos;

    private float shakeTimer = 0f;
    private float shakeTimeRemaining = 0f;

    void Start()
    {
        if (normalObject != null)
        {
            normalObject.SetActive(true);
            originalShelfPos = normalObject.transform.localPosition;
        }

        if (mannequinObject != null)
        {
            mannequinObject.SetActive(false);
        }

        ResetShakeTimer();
    }

    void Update()
    {
        if (player == null || VisionManager.Instance == null) return;

        HandleShelfInversion();
        HandleShelfShake();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isTransformed && distanceToPlayer <= triggerDistance)
        {
            TransformToMannequin();
        }

        if (isTransformed)
        {
            ChasePlayer();
        }
    }

    void HandleShelfInversion()
    {
        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (normalObject != null && shouldBeInverted != isShelfUpsideDown)
        {
            isShelfUpsideDown = shouldBeInverted;
            Vector3 euler = normalObject.transform.eulerAngles;
            euler.z = isShelfUpsideDown ? 180f : 0f;
            normalObject.transform.eulerAngles = euler;
        }
    }

    void HandleShelfShake()
    {
        if (normalObject == null || isTransformed) return;

        if (shakeTimeRemaining > 0f)
        {
            Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f,
                0f
            ) * shakeAmount;

            normalObject.transform.localPosition = originalShelfPos + shakeOffset;
            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            normalObject.transform.localPosition = originalShelfPos;
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

    void TransformToMannequin()
    {
        if (normalObject != null) normalObject.SetActive(false);

        if (mannequinObject != null)
        {
            mannequinObject.SetActive(true);

            // ここを修正！ローカル回転で反転！
            mannequinObject.transform.localEulerAngles = new Vector3(
                mannequinObject.transform.localEulerAngles.x,
                mannequinObject.transform.localEulerAngles.y,
                180f
            );
        }

        isTransformed = true;
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                5f * Time.deltaTime
            );
        }
    }
}