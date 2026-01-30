using UnityEngine;
using UnityEngine.SceneManagement;

public class Benemy1 : MonoBehaviour
{
    [Header("反転対象")]
    public Transform normalObject;

    [Header("マネキン")]
    public GameObject mannequinObject;
    public Animator mannequinAnimator;

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
        invertedRot = Quaternion.Euler(
            defaultRot.eulerAngles.x,
            defaultRot.eulerAngles.y,
            defaultRot.eulerAngles.z + 180f
        );

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

        // normalObject が破壊されたらマネキンも破壊
        if (normalObject == null)
        {
            if (mannequinObject != null)
            {
                Destroy(mannequinObject);
            }
            Destroy(gameObject); // このスクリプトを持つオブジェクトも破壊
            return;
        }

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

            Debug.Log("Mannequin Appear");

            if (mannequinAnimator != null)
            {
                mannequinAnimator.Play("Appear", 0, 0f);
            }

            if (normalObject != null)
            {
                normalObject.gameObject.SetActive(false);
            }
        }

        if (shelfShaker != null)
        {
            shelfShaker.enabled = !shouldInvert;
        }

        // ★★★ 追跡処理 ★★★
        if (hasBeenShown) // 出現後のみ追跡
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, mannequinObject.transform.position);

            if (distanceToPlayer <= chaseDistance)
            {
                // プレイヤー方向へ移動
                Vector3 direction = (playerTransform.position - mannequinObject.transform.position).normalized;
               

                mannequinObject.transform.position += direction * moveSpeed * Time.deltaTime;

                // プレイヤーの方向を向く
                mannequinObject.transform.rotation = Quaternion.LookRotation(direction);
            }
        }


    }


    // ★★★ 追加：トリガーでプレイヤー即死 ★★★
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Killed by Mannequin");
            SceneManager.LoadScene("GameOver");
        }
    }

    void OnDestroy()
    {
        if (normalObject != null)
            Destroy(normalObject.gameObject);

        if (mannequinObject != null)
            Destroy(mannequinObject);
    }
}