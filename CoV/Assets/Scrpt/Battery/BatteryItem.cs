using UnityEngine;

public class BatteryItem : MonoBehaviour
{
    [Header("設定")]
    public float pickupRange = 3f; // プレイヤーがこの距離以内で取得可能
    public KeyCode pickupKey = KeyCode.E; // キーボード操作（デバッグ用）

    [Header("エフェクト関連（任意）")]
    public GameObject pickupEffect; // 回収時エフェクト（任意）

    private VisionManager visionManager;
    private Transform playerTransform;

    void Start()
    {
        visionManager = VisionManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("⚠ Playerタグのオブジェクトが見つかりません。BatteryItemが動作しません。");
    }

    void Update()
    {
        if (playerTransform == null || visionManager == null) return;

        // プレイヤーとの距離をチェック
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= pickupRange)
        {
            // XboxコントローラーのAボタン or Eキーで取得
            if (Input.GetKeyDown(pickupKey) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                RecoverAllVisions();
                HandlePickup();
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
        // エフェクトが設定されていれば再生
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // 自身を削除
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // シーンビューで取得範囲を可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
