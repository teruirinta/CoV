using UnityEngine;

public class BallThrower : MonoBehaviour
{
    [Header("粉の設定")]
    public ParticleSystem powderEffectPrefab;
    public Transform throwPoint;
    public int maxStock = 10;
    private int currentStock = 10;

    [Header("エイム設定")]
    public float zoomFOV = 30f;
    public float zoomSpeed = 5f;

    private bool isAiming = false;
    private Camera mainCamera;
    private float normalFOV;

    // ★追加：トリガーが押しっぱなしの状態かを記録する変数
    private bool isRTDown = false;

    void Start()
    {
        mainCamera = Camera.main;
        normalFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        float lt = Input.GetAxis("LT");
        float rt = Input.GetAxis("RT");

        // エイム処理
        if (lt > 0.2f || Input.GetMouseButton(1) || Input.GetButton("LT"))
        {
            isAiming = true;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            isAiming = false;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
        }

        // =========================
        // 粉を投げる（修正ポイント）
        // =========================

        // ★RTが一定以上押し込まれていて、かつ「前のフレームでは押されていなかった」場合のみ実行
        bool rtJustPressed = false;
        if (rt > 0.5f)
        {
            if (!isRTDown) // まだ「押しっぱなし状態」として記録されていなければ
            {
                rtJustPressed = true; // 今回のフレームで初めて押されたと判定
                isRTDown = true;      // 押しっぱなし状態にセット
            }
        }
        else
        {
            isRTDown = false; // トリガーを離したらリセット
        }

        // マウスの左クリック、またはRTが新しく押された瞬間
        if (isAiming && (rtJustPressed || Input.GetMouseButtonDown(0)))
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

    void ThrowPowder()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        Vector3 direction = ray.direction;
        Quaternion rotation = Quaternion.LookRotation(direction);

        ParticleSystem powder = Instantiate(powderEffectPrefab, throwPoint.position, rotation);
        powder.Play();

        Destroy(powder.gameObject, powder.main.duration + powder.main.startLifetime.constantMax);

        Rigidbody rb = powder.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float throwForce = 10f;
            rb.linearVelocity = direction * throwForce;
        }
    }

    public void AddStock(int amount)
    {
        currentStock = Mathf.Min(currentStock + amount, maxStock);
    }

    public int CurrentStock => currentStock;
}