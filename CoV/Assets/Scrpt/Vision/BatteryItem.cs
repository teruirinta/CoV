using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BatteryItem : MonoBehaviour
{
    [Header("設定")]
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("エフェクト・音響")]
    public GameObject pickupEffect;
    public AudioClip pickupSound; // 回復時の効果音
    [Range(0f, 1f)] public float volume = 1f;

    private VisionManager visionManager;
    private Transform playerTransform;

    void Start()
    {
        visionManager = VisionManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("⚠ Playerタグのオブジェクトが見つかりません。");
    }

    void Update()
    {
        if (playerTransform == null || visionManager == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // プレイヤーがこっちを向いているかチェック
        Vector3 toBattery = (transform.position - playerTransform.position).normalized;
        float dot = Vector3.Dot(playerTransform.forward, toBattery);
        bool isLookingAt = dot > 0.7f;

        if (distance <= pickupRange && isLookingAt)
        {
            // 向いているときだけ拾える
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

        // --- 効果音の再生 ---
        if (pickupSound != null)
        {
            // アイテムが消えても音が鳴るように、カメラの位置で再生
            AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, volume);
        }

        Debug.Log("🔋 全視界のバッテリーを全回復 & SE再生");
    }

    void HandlePickup()
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // 自身を削除
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}