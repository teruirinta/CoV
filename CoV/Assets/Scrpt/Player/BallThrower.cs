using UnityEngine;

public class BallThrower : MonoBehaviour
{
    public ParticleSystem powderEffectPrefab; // 粉のパーティクルプレハブ
    public Transform throwPoint;
    public int maxStock = 100;
    private int currentStock = 10000;

    private bool isAiming = false;

    public float zoomFOV = 30f;
    public float zoomSpeed = 5f;

    private Camera mainCamera;
    private float normalFOV;

    void Start()
    {
        mainCamera = Camera.main;
        normalFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        // ★ Xbox LT 入力（旧 Input Manager）
        float lt = Input.GetAxis("LT");   // 0〜1（または -1〜1 の場合あり）

        // ★ Xbox RT 入力（旧 Input Manager）
        float rt = Input.GetAxis("RT");

        // ■ エイム（LT または マウス右）

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

        // ■ 粉を投げる（RT または マウス左）

        if (isAiming && (rt > 0.5f || Input.GetMouseButtonDown(0) || Input.GetButtonDown("RT")))
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
        Debug.Log("粉を補充！現在のストック: " + currentStock);
    }
}