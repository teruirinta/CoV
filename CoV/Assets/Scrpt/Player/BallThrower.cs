using UnityEngine;

public class BallThrower : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform throwPoint;
    public float throwForce = 700f;
    public int maxStock = 100;
    private int currentStock = 100;

    private bool isAiming = false;

    public float zoomFOV = 30f;
    public float zoomSpeed = 5f;

    private Camera mainCamera;
    private float normalFOV;

    void Start()
    {
        mainCamera = Camera.main;
        normalFOV = mainCamera.fieldOfView; // Unityの初期設定をそのまま使う！
    }

    void Update()
    {
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

        if (isAiming && Input.GetMouseButtonDown(0))
        {
            if (currentStock > 0)
            {
                ThrowBall();
                currentStock--;
                Debug.Log("残り玉: " + currentStock);
            }
            else
            {
                Debug.Log("玉がないよ〜！");
            }
        }
    }

    void ThrowBall()
    {
        GameObject ball = Instantiate(ballPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Vector3 throwDirection = Camera.main.transform.forward;
        rb.AddForce(throwDirection * throwForce);
    }

    public void AddStock(int amount)
    {
        currentStock = Mathf.Min(currentStock + amount, maxStock);
        Debug.Log("玉を補充！現在のストック: " + currentStock);
    }
}
