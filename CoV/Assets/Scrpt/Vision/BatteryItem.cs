using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BatteryItem : MonoBehaviour
{
    [Header("設定")]
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("エフェクト・音響")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    [Range(0f, 1f)] public float volume = 1f;

    private VisionManager visionManager;
    private Transform playerTransform;
    private bool isRecovered = false; // 二重回復防止フラグ

    void Start()
    {
        visionManager = VisionManager.Instance;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null || visionManager == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // ★反転対策：体の向きではなくカメラの向きで判定
        Vector3 toBattery = (transform.position - Camera.main.transform.position).normalized;
        float dot = Vector3.Dot(Camera.main.transform.forward, toBattery);
        bool isLookingAt = dot > 0.6f;

        if (distance <= pickupRange && isLookingAt)
        {
            if (Input.GetKeyDown(pickupKey) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                // player.cs側でDestroyされても、ここで一度Recoverを呼んでおけば確実
                RecoverAllVisions();
                HandlePickup();
            }
        }
    }

    public void RecoverAllVisions()
    {
        if (isRecovered) return; // すでに回復済みなら無視
        isRecovered = true;

        if (visionManager == null) return;

        foreach (var data in visionManager.visionDataList)
        {
            data.currentBattery = data.maxBattery;
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, volume);
        }

        Debug.Log($"🔋 {gameObject.name}: 全回復を実行しました");
    }

    // ★修正のキモ：どのような理由で消されても、消える直前に回復を試みる
    private void OnDestroy()
    {
        // まだ回復していない（＝player.csなどによって直接消された）場合、ここで回復させる
        if (!isRecovered && visionManager != null)
        {
            RecoverAllVisions();
            // Destroy中なのでInstantiateはできないためエフェクトは省略
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
}