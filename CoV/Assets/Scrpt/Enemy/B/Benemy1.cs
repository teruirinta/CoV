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
    public float chaseDistance = 5f; // 追跡開始距離
    public float moveSpeed = 2f;

    public bool isInverted = false;
    private Quaternion defaultRot;
    private Quaternion invertedRot;
    private Vector3 defaultPos;
    private Vector3 invertedPos;

    [Header("反転時の高さ補正")]
    public float verticalOffset; // 埋まり防止の高さ調整

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
            mannequinObject.SetActive(false); // 初期状態は非表示
        }
    }

    void Update()
    {
        if (VisionManager.Instance == null || normalObject == null) return;

        bool shouldInvert = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldInvert != isInverted)
        {
            isInverted = shouldInvert;
            normalObject.localRotation = isInverted ? invertedRot : defaultRot;
            normalObject.localPosition = isInverted ? invertedPos : defaultPos;
        }

        if (mannequinObject == null || playerTransform == null) return;

        float distance = Vector3.Distance(playerTransform.position, mannequinObject.transform.position);
        bool shouldShow = distance <= showDistance;

        if (mannequinObject.activeSelf != shouldShow)
        {
            mannequinObject.SetActive(shouldShow);
        }
        if (shelfShaker != null)
        {
            shelfShaker.enabled = !shouldInvert;
        }
        if (mannequinObject.activeSelf && distance <= chaseDistance)
        {
            Vector3 direction = (playerTransform.position - mannequinObject.transform.position).normalized;
            mannequinObject.transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}