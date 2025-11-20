using UnityEngine;
using UnityEngine.SceneManagement;


public class KeyPickup : MonoBehaviour
{
    [Header("設定")]
    public string keyId = "EscapeKey";
    public AudioClip pickupSound;
    public ParticleSystem pickupEfect;
    public bool autoSaveOnPickup = true;
    public float pickupRange = 3f;
    public float viewAngleThreshold = 30f; // 視線の角度許容範囲

    private GameObject player;
    private Camera playerCamera;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerCamera = player.GetComponentInChildren<Camera>();

        if (playerCamera == null)
            Debug.LogWarning("[KeyPickup] プレイヤーのカメラが見つかりません！");
    }

    private void Update()
    {
        if (player == null || playerCamera == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > pickupRange) return;

        Vector3 toKey = (transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, toKey);

        if (angle <= viewAngleThreshold && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogWarning("[KeyPickup] PlayerInventory が見つかりません。");
            return;
        }

        inv.AddKey(keyId);

        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);



        if (pickupEfect)
        {
            var effect = Instantiate(pickupEfect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration);
        }

        if (autoSaveOnPickup)
            SaveManager.Instance?.SaveKeyObtained(keyId);

        EnemySpawner.Instance?.OnKeyPickedUp(keyId);

        Destroy(gameObject);
    }
}
