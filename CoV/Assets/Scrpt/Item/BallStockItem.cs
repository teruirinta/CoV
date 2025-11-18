using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(QuickOutline))]
public class BallStockItem : MonoBehaviour
{
    [Header("設定")]
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("エフェクト関連（任意）")]
    public GameObject pickupEffect;

    private Transform playerTransform;
    private QuickOutline outline;
    private bool isHighlighted = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("⚠ Playerタグのオブジェクトが見つかりません。BallStockItemが動作しません。");

        outline = GetComponent<QuickOutline>();
        outline.enabled = false;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= pickupRange)
        {
            if (!isHighlighted)
            {
                outline.enabled = true;
                isHighlighted = true;
            }

            if (Input.GetKeyDown(pickupKey) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                AddBallStock();
                HandlePickup();
            }
        }
        else
        {
            if (isHighlighted)
            {
                outline.enabled = false;
                isHighlighted = false;
            }
        }
    }

    void AddBallStock()
    {
        BallThrower thrower = playerTransform.GetComponent<BallThrower>();
        if (thrower != null)
        {
            thrower.AddStock(1);
            Debug.Log(" 玉を1個補充したよ！");
        }
        else
        {
            Debug.LogWarning("BallThrowerがプレイヤーに見つかりませんでした！");
        }
    }

    void HandlePickup()
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
