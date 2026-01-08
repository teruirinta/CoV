using UnityEngine;

public class BallThrower : MonoBehaviour
{
    [Header("粉の設定")]
    public ParticleSystem powderEffectPrefab;   // 粉のパーティクル
    public Transform throwPoint;                 // 投げる位置
    public int maxStock = 10;                    // 最大所持数
    private int currentStock = 10;               // 現在の所持数

    [Header("エイム設定")]
    public float zoomFOV = 30f;                  // エイム時FOV
    public float zoomSpeed = 5f;                 // ズーム速度

    private bool isAiming = false;

    private Camera mainCamera;
    private float normalFOV;

    void Start()
    {
        mainCamera = Camera.main;
        normalFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        // Xbox LT / RT（旧InputManager）
        float lt = Input.GetAxis("LT");
        float rt = Input.GetAxis("RT");

        // =========================
        // エイム処理（LT / 右クリック）
        // =========================
        if (lt > 0.2f || Input.GetMouseButton(1) || Input.GetButton("LT"))
        {
            isAiming = true;
            mainCamera.fieldOfView = Mathf.Lerp(
                mainCamera.fieldOfView,
                zoomFOV,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            isAiming = false;
            mainCamera.fieldOfView = Mathf.Lerp(
                mainCamera.fieldOfView,
                normalFOV,
                Time.deltaTime * zoomSpeed
            );
        }

        // =========================
        // 粉を投げる（RT / 左クリック）
        // =========================
        if (isAiming &&
            (rt > 0.5f || Input.GetMouseButtonDown(0) || Input.GetButtonDown("RT")))
        {
            if (currentStock > 0)
            {
                ThrowPowder();
                currentStock--;
                Debug.Log("残り粉: " + currentStock);
            }
            else
            {
                Debug.Log("粉がないよ〜！");
            }
        }
    }

    // =========================
    // 粉を投げる処理
    // =========================
    void ThrowPowder()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
        );

        Vector3 direction = ray.direction;
        Quaternion rotation = Quaternion.LookRotation(direction);

        ParticleSystem powder =
            Instantiate(powderEffectPrefab, throwPoint.position, rotation);

        powder.Play();

        // パーティクル終了後に削除
        Destroy(
            powder.gameObject,
            powder.main.duration + powder.main.startLifetime.constantMax
        );

        // 前方向に飛ばす
        Rigidbody rb = powder.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float throwForce = 10f;
            rb.linearVelocity = direction * throwForce;   // ← Unity標準
        }
    }

    // =========================
    // ストック補充（アイテム用）
    // =========================
    public void AddStock(int amount)
    {
        currentStock = Mathf.Min(currentStock + amount, maxStock);
        Debug.Log("粉を補充！現在のストック: " + currentStock);
    }

    // =========================
    // UI表示用（読み取り専用）
    // =========================
    public int CurrentStock
    {
        get { return currentStock; }
    }
}