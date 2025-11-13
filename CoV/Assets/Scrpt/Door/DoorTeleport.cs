using UnityEngine;
using System.Collections;

public class DoorTeleport : MonoBehaviour
{
    public Transform player;
    public Transform teleportTarget;
    public float activationDistance = 0.5f;
    public float teleportCooldown = 1.5f;

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

        // ドアを見ているかどうかを判定
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

        // テレポート条件判定
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

                if (controller != null) controller.enabled = false;

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

            // 次のフレームでCharacterControllerを再有効化
            StartCoroutine(ReenableControllerNextFrame());

            if (VisionManager.Instance != null)
                VisionManager.Instance.IsTeleporting = isTeleported;
        }
    }

    IEnumerator ReenableControllerNextFrame()
    {
        yield return null;
        if (controller != null) controller.enabled = true;
    }
}
