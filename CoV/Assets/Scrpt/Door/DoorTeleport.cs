using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform player;
    public Transform teleportTarget;
    public float activationDistance = 0.5f;
    public float teleportCooldown = 1.5f; // クールタイム（秒）

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTeleported = false;
    private CharacterController controller;
    private float lastTeleportTime = -Mathf.Infinity;

    void Start()
    {
        controller = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // ドアを見ているかどうかを判定（テレポートを防ぐため）
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        bool isLookingAtDoor = false;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.collider.GetComponent<OpenDoor>() != null)
            {
                isLookingAtDoor = true;
            }
        }

        // クールタイム判定とドア優先判定
        if (distance <= activationDistance &&
            Input.GetKeyDown(KeyCode.E) &&
            !isLookingAtDoor &&
            Time.time - lastTeleportTime >= teleportCooldown)
        {
            lastTeleportTime = Time.time;

            if (!isTeleported)
            {
                originalPosition = player.position;
                originalRotation = player.rotation;

                if (controller != null) controller.enabled = false; // CharacterControllerを無効化

                player.position = teleportTarget.position;
                player.rotation = teleportTarget.rotation;

                isTeleported = true;
            }
            else
            {
                if (controller != null) controller.enabled = false;

                player.position = originalPosition;
                player.rotation = originalRotation;

                isTeleported = false;
            }

            // テレポート状態に応じてCharacterControllerを切り替え
            if (controller != null) controller.enabled = !isTeleported;

            if (VisionManager.Instance != null)
                VisionManager.Instance.IsTeleporting = isTeleported;
        }
    }
}
