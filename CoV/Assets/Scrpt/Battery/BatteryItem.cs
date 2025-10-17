using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(QuickOutline))]
public class BatteryItem : MonoBehaviour
{
    [Header("設定")]
    public float pickupRange = 3f; // プレイヤーがこの距離以内で取得可能
    public KeyCode pickupKey = KeyCode.E; // キーボード操作（デバッグ用）

    [Header("エフェクト関連（任意）")]
    public GameObject pickupEffect; // 回収時エフェクト（任意）

    private VisionManager visionManager;
    private Transform playerTransform;
    private QuickOutline outline;

    private bool isHighlighted = false; // 現在アウトラインが表示中かどうか

    void Start()
    {
        visionManager = VisionManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("⚠ Playerタグのオブジェクトが見つかりません。BatteryItemが動作しません。");

        outline = GetComponent<QuickOutline>();
        outline.enabled = false; // 初期状態では非表示
    }

    void Update()
    {
        if (playerTransform == null || visionManager == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 範囲内ならアウトラインON、範囲外ならOFF
        if (distance <= pickupRange)
        {
            if (!isHighlighted)
            {
                outline.enabled = true;
                isHighlighted = true;
            }

            // Aボタン or Eキーで取得
            if (Input.GetKeyDown(pickupKey) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                RecoverAllVisions();
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

    void RecoverAllVisions()
    {
        foreach (var data in visionManager.visionDataList)
        {
            data.currentBattery = data.maxBattery;
        }

        Debug.Log("🔋 全視界のバッテリーを全回復しました！");
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
