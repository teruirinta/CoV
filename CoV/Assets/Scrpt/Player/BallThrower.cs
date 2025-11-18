using UnityEngine;

public class BallThrower : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform throwPoint;
    public float throwForce = 700f;
    public int maxStock;
    private int currentStock = 100; // 最初は2個！

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
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

    // アイテム取得時に呼び出す
    public void AddStock(int amount)
    {
        currentStock = Mathf.Min(currentStock + amount, maxStock);
        Debug.Log("玉を補充！現在のストック: " + currentStock);
    }
}
