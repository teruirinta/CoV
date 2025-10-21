using UnityEngine;

public class DoorTeleportToggle : MonoBehaviour
{
    public Transform player;              // プレイヤーのTransform（CharacterControllerがついてるオブジェクト）
    public Transform teleportTarget;      // ワープ先のTransform
    public float activationDistance = 0.5f; // ドアに近づいたときだけ反応

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTeleported = false;
    private CharacterController controller;

    void Start()
    {
        controller = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTeleported)
            {
                originalPosition = player.position;
                originalRotation = player.rotation;

                if (controller != null) controller.enabled = false;
                player.position = teleportTarget.position;
                player.rotation = teleportTarget.rotation;
                if (controller != null) controller.enabled = true;

                isTeleported = true;
            }
            else
            {
                if (controller != null) controller.enabled = false;
                player.position = originalPosition;
                player.rotation = originalRotation;
                if (controller != null) controller.enabled = true;

                isTeleported = false;
            }

            // ✅ TP状態を VisionManager に通知
            if (VisionManager.Instance != null)
                VisionManager.Instance.IsTeleporting = isTeleported;
        }
    }
}
