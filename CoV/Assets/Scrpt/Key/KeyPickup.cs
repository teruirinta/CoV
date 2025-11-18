using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [Header("ê›íË")]
    public string keyId = "EscapeKey";
    public AudioClip pickupSound;
    public ParticleSystem pickupEfect;
    public bool autoSaveOnPickup = true;

    private bool isPlayerNearby = false;
    private GameObject player;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
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

    private void TryPickup()
    {
        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogWarning("[KeyPickup] PlayerInventory Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB");
            return;
        }

        inv.AddKey(keyId);

        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        if (pickupEfect)
            Instantiate(pickupEfect, transform.position, Quaternion.identity);

        if (autoSaveOnPickup)
            SaveManager.Instance?.SaveKeyObtained(keyId);

        EnemySpawner.Instance?.OnKeyPickedUp(keyId);

        Destroy(gameObject);
    }
}
