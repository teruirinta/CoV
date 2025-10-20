using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour
{
    [Header("設定")]
    public string requiredKeyId = "EscapeKey"; // この扉を開けるのに必要な鍵のID
    public bool consumeKeyOnOpen = true;       // 扉を開けたときに鍵を消費するか
    public float openDuration = 1.0f;          // 開くまでの時間
    public Animator doorAnimator;              // 開閉アニメーションがある場合は割り当て

    private bool isOpen = false;               // 扉が開いているかどうか

    private void Reset()
    {
        // 自動で Trigger に設定
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 既に開いているなら何もしない
        if (isOpen) return;

        // Player 以外は無視
        if (!other.CompareTag("Player")) return;

        // PlayerInventory を取得
        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        // 鍵を持っていて、かつセーブに記録されているかチェック
        if (inv.HasKey && SaveManager.Instance != null && SaveManager.Instance.IsKeySaved(requiredKeyId))
        {
            // 鍵があるので開ける
            StartCoroutine(OpenDoor(inv));
        }
        else
        {
            // 鍵がない場合のフィードバック（音やメッセージを出しても良い）
            Debug.Log("[Door] 鍵が必要です。");
        }
    }

    private IEnumerator OpenDoor(PlayerInventory inv)
    {
        isOpen = true;
        Debug.Log("[Door] 扉を開けます。");

        // アニメーション再生
        if (doorAnimator) doorAnimator.SetTrigger("Open");

        // 開くまで待つ
        yield return new WaitForSeconds(openDuration);

        // 鍵を使う設定なら消費
        if (consumeKeyOnOpen)
        {
            inv.UseKey();
            SaveManager.Instance?.ConsumeKey(requiredKeyId);
        }

        // 扉を物理的に開放（コライダー無効化など）
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // ゲームクリア処理を呼ぶ（例：ステージ遷移）
        GameController.Instance?.OnPlayerEscaped();

        Debug.Log("[Door] 扉が開きました。");
    }
}