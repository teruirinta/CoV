using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour
{
    [Header("設定")]
    public string requiredKeyId = "EscapeKey";
    public bool consumeKeyOnOpen = true;
    public float openDuration = 1.0f;
    public Animator doorAnimator;

    private bool isOpen = false;
    private bool playerInRange = false;
    private PlayerInventory currentPlayer;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (currentPlayer != null && currentPlayer.HasKey &&
                SaveManager.Instance != null &&
                SaveManager.Instance.IsKeySaved(requiredKeyId))
            {
                StartCoroutine(OpenDoor(currentPlayer));
            }
            else
            {
                Debug.Log("[Door] 鍵が必要です。");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        currentPlayer = other.GetComponent<PlayerInventory>();
        if (currentPlayer != null)
        {
            playerInRange = true;
            Debug.Log("[Door] 扉の前に来ました。Eキーで開けられます。");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.GetComponent<PlayerInventory>() == currentPlayer)
        {
            playerInRange = false;
            currentPlayer = null;
        }
    }

    private IEnumerator OpenDoor(PlayerInventory inv)
    {
        isOpen = true;
        Debug.Log("[Door] 扉を開けます。");

        if (doorAnimator) doorAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(openDuration);

        if (consumeKeyOnOpen)
        {
            inv.UseKey();
            SaveManager.Instance?.ConsumeKey(requiredKeyId);
        }

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        //SceneManager.LoadScene("Goal");

        Debug.Log("[Door] 扉が開きました。");
    }
}
