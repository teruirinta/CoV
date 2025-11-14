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
    private bool isReturning = false; // 戻り中フラグ

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

        float distance = Vector3.Distance(playerTransform.position, mannequinObject.transform.position);

        // 一度表示されたら表示状態にする
        if (!hasBeenShown && distance <= showDistance)
        {
            mannequinObject.SetActive(true);
            hasBeenShown = true;

            if (normalObject != null)
            {
                Destroy(normalObject.gameObject);
                normalObject = null;
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
                Vector3 direction = directionToPlayer.normalized;
                mannequinObject.transform.position += direction * moveSpeed * Time.deltaTime;
                isReturning = false;
            }
            else
            {
                // 戻る処理
                mannequinObject.transform.position = Vector3.MoveTowards(
                    mannequinObject.transform.position,
                    mannequinStartPos,
                    moveSpeed * Time.deltaTime
                );
                isReturning = true;

                // 完全に戻ったら非表示＆リセット
                if (Vector3.Distance(mannequinObject.transform.position, mannequinStartPos) < 0.01f)
                {
                    mannequinObject.SetActive(false);
                    hasBeenShown = false;
                    isReturning = false;
                }
            }
        }
    }
}
