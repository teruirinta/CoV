using UnityEngine;

public class DoorTeleportToggle : MonoBehaviour
{
    public Transform player;               // プレイヤーのTransform（CharacterControllerがついてるオブジェクト）
    public Transform teleportTarget;       // ワープ先のTransform
    public float activationDistance = 3f;  // ドアに近づいたときだけ反応

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
                // 元の位置を保存
                originalPosition = player.position;
                originalRotation = player.rotation;

                // テレポート
                if (controller != null) controller.enabled = false;
                player.position = teleportTarget.position;
                player.rotation = teleportTarget.rotation;
                if (controller != null) controller.enabled = true;

                isTeleported = true;
            }
            else
            {
                // 元の位置に戻る
                if (controller != null) controller.enabled = false;
                player.position = originalPosition;
                player.rotation = originalRotation;
                if (controller != null) controller.enabled = true;

                isTeleported = false;
            }
        }
    }
}