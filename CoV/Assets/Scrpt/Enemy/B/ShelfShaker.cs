using UnityEngine;

public class ShelfShaker : MonoBehaviour
{
    [Header("震える対象")]
    public Transform targetObject;

    [Header("棚の見た目")]
    public MeshRenderer shelfVisual;

    [Header("プレイヤー")]
    public Transform player;
    public float breakDistance = 2f;

    [Header("ガタガタ設定")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 10f;
    public float minShakeInterval = 2f;
    public float maxShakeInterval = 5f;
    public float shakeDuration = 1f;

    [Header("戻る速度")]
    public float moveSpeed = 1f;

    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float shakeTimeRemaining = 0f;
    private bool isHidden = false;

    void Start()
    {
        if (targetObject != null)
        {
            originalPos = targetObject.localPosition;
        }

        if (shelfVisual == null)
        {
            shelfVisual = targetObject.GetComponentInChildren<MeshRenderer>();
        }

        ResetShakeTimer();
    }

    void Update()
    {
        if (targetObject == null || shelfVisual == null) return;

        float distanceToPlayer = player != null ? Vector3.Distance(player.position, transform.position) : Mathf.Infinity;

        // プレイヤーが近づいたら見た目を非表示
        if (!isHidden && distanceToPlayer < breakDistance)
        {
            HideShelf();
            return;
        }

        // 非表示状態なら元の位置に戻す
        if (isHidden)
        {
            targetObject.localPosition = Vector3.MoveTowards(
                targetObject.localPosition,
                originalPos,
                moveSpeed * Time.deltaTime
            );

            // 戻ったら再表示
            if (Vector3.Distance(targetObject.localPosition, originalPos) < 0.01f)
            {
                shelfVisual.enabled = true;
                isHidden = false;
                ResetShakeTimer();
                Debug.Log("棚が再表示された！");
            }

            return;
        }

        // 揺れ処理
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

    void HideShelf()
    {
        shelfVisual.enabled = false;
        isHidden = true;
        Debug.Log("棚が非表示になった！");
    }
}
