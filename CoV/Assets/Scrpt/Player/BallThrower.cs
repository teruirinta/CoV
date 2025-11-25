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
        // エイム中のズーム処理
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            isAiming = false;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
        }

        // 粉を投げる処理
        if (isAiming && Input.GetMouseButtonDown(0))
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
        // 画面中央からレイを飛ばす
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        Vector3 direction = ray.direction;
        Quaternion rotation = Quaternion.LookRotation(direction);

        ParticleSystem powder = Instantiate(powderEffectPrefab, throwPoint.position, rotation);
        powder.Play();

        // パーティクルの寿命に合わせて削除
        Destroy(powder.gameObject, powder.main.duration + powder.main.startLifetime.constantMax);

        // Rigidbodyがある場合は速度を設定
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
