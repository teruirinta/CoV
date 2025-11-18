using UnityEngine;

public class Benemy1 : MonoBehaviour
{
    [Header("反転対象")]
    public Transform normalObject;

    [Header("マネキン")]
    public GameObject mannequinObject;

    [Header("プレイヤー")]
    public Transform playerTransform;

    [Header("表示距離")]
    public float showDistance;

    [Header("棚の揺れスクリプト")]
    public ShelfShaker shelfShaker;

    [Header("追跡設定")]
    public float chaseDistance = 5f;
    public float moveSpeed = 2f;

    public bool isInverted = false;
    private Quaternion defaultRot;
    private Quaternion invertedRot;
    private Vector3 defaultPos;
    private Vector3 invertedPos;
    private bool hasBeenShown = false;

    [Header("反転時の高さ補正")]
    public float verticalOffset;

    private Vector3 mannequinStartPos;
    private bool isReturning = false;

    private Rigidbody mannequinRb;

    void Start()
    {
        if (normalObject == null)
        {
            Debug.LogWarning("normalObject が設定されていません！");
            return;
        }

        defaultRot = normalObject.localRotation;
        invertedRot = Quaternion.Euler(defaultRot.eulerAngles.x, defaultRot.eulerAngles.y, defaultRot.eulerAngles.z + 180f);

        defaultPos = normalObject.localPosition;
        invertedPos = defaultPos + new Vector3(0, verticalOffset, 0);

        if (mannequinObject != null)
        {
            mannequinObject.SetActive(false);
            mannequinStartPos = mannequinObject.transform.position;
            mannequinRb = mannequinObject.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (VisionManager.Instance == null) return;

        bool shouldInvert = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (normalObject != null && shouldInvert != isInverted)
        {
            isInverted = shouldInvert;
            normalObject.localRotation = isInverted ? invertedRot : defaultRot;
            normalObject.localPosition = isInverted ? invertedPos : defaultPos;
        }

        if (mannequinObject == null || playerTransform == null) return;

        Vector3 offset = playerTransform.position - mannequinObject.transform.position;
        offset.y *= 0.5f;
        float adjustedDistance = offset.magnitude;

        if (!hasBeenShown && adjustedDistance <= showDistance)
        {
            mannequinObject.SetActive(true);
            hasBeenShown = true;

            if (normalObject != null)
            {
                normalObject.gameObject.SetActive(false);
            }
        }

        if (shelfShaker != null)
        {
            shelfShaker.enabled = !shouldInvert;
        }

        if (mannequinObject.activeSelf)
        {
            bool canSeePlayer = false;

            Vector3 directionToPlayer = playerTransform.position - mannequinObject.transform.position;
            Ray ray = new Ray(mannequinObject.transform.position, directionToPlayer.normalized);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == playerTransform)
                {
                    canSeePlayer = true;
                }
            }

            if (canSeePlayer)
            {
                Vector3 direction;

                if (shouldInvert)
                {
                    direction = directionToPlayer.normalized;

                    if (mannequinRb.useGravity)
                    {
                        mannequinRb.useGravity = false;
                    }
                }
                else
                {
                    Vector3 flatDirection = directionToPlayer;
                    flatDirection.y = 0;
                    direction = flatDirection.normalized;

                    if (!mannequinRb.useGravity)
                    {
                        mannequinRb.useGravity = true;
                    }
                }

                mannequinRb.MovePosition(mannequinRb.position + direction * moveSpeed * Time.deltaTime);
                isReturning = false;
            }
            else
            {
                if (!mannequinRb.useGravity)
                {
                    mannequinRb.useGravity = true;
                }

                Vector3 returnPos = Vector3.MoveTowards(
                    mannequinRb.position,
                    mannequinStartPos,
                    moveSpeed * Time.deltaTime
                );
                mannequinRb.MovePosition(returnPos);

                isReturning = true;

                if (Vector3.Distance(mannequinObject.transform.position, mannequinStartPos) < 0.01f)
                {
                    mannequinObject.SetActive(false);
                    hasBeenShown = false;
                    isReturning = false;

                    if (normalObject != null)
                    {
                        normalObject.gameObject.SetActive(true);
                    }
                }
            }
        }

        if (normalObject != null && mannequinObject.activeSelf)
        {
            float moveDistance = Vector3.Distance(mannequinObject.transform.position, mannequinStartPos);
            if (moveDistance > 0.01f)
            {
                normalObject.gameObject.SetActive(false);
            }
            else if (!normalObject.gameObject.activeSelf)
            {
                normalObject.gameObject.SetActive(true);
            }
        }
    }
}
