using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour
{
    [Header("設定")]
    public string requiredKeyId = "EscapeKey";
    public string sceneToLoad = "NextScene";

    private bool isPlayerNearby = false;
    private GameObject player;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Jump"))
        {
            TryOpenDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }

    private void TryOpenDoor()
    {
        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogWarning("[DoorController] PlayerInventory が見つかりません。");
            return;
        }

        if (inv.HasKey(requiredKeyId))
        {
            Debug.Log($"[DoorController] 鍵「{requiredKeyId}」が使われました。シーンを切り替えます！");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log($"[DoorController] 鍵「{requiredKeyId}」が必要です！");
        }
    }
}
