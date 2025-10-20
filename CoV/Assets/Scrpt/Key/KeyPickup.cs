using UnityEngine;

[RequireComponent(typeof(Collider))] // Collider が必須であることを指定
public class KeyPickup : MonoBehaviour
{
    [Header("設定")]
    public string keyId = "EscapeKey"; // 複数の鍵を区別したいとき用のID
    public AudioClip pickupSound;         // 鍵を拾った時の音
    public ParticleSystem pickupEfect;    // 鍵を拾った時のエフェクト
    public bool autoSaveOnPickup = true; // 拾った瞬間にセーブするか

    private void Reset()
    {
        // 自動で isTrigger にしておく
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player 以外のオブジェクトは無視
        if (!other.CompareTag("Player")) return;

        // PlayerInventory（持ち物管理）を取得
        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogWarning("[KeyPickup] PlayerInventory が見つかりません。");
            return;
        }

        // 鍵を追加
        inv.AddKey();

        // 効果音再生
        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // エフェクト再生
        if (pickupEfect) Instantiate(pickupEfect, transform.position, Quaternion.identity);

        // セーブデータに記録
        if (autoSaveOnPickup)
        {
            SaveManager.Instance?.SaveKeyObtained(keyId);
        }

        // 鍵を拾ったことを敵スポーナーに通知
        // （敵がいない場合は無視される）
        EnemySpawner.Instance?.OnKeyPickedUp(keyId);

        // 鍵オブジェクトを消す
        Destroy(gameObject);
    }
}