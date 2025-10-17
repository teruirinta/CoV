using UnityEngine;

public class DoorTeleportToggle : MonoBehaviour
{
    public Transform player;               // �v���C���[��Transform�iCharacterController�����Ă�I�u�W�F�N�g�j
    public Transform teleportTarget;       // ���[�v���Transform
    public float activationDistance = 3f;  // �h�A�ɋ߂Â����Ƃ���������

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
                // ���̈ʒu��ۑ�
                originalPosition = player.position;
                originalRotation = player.rotation;

                // �e���|�[�g
                if (controller != null) controller.enabled = false;
                player.position = teleportTarget.position;
                player.rotation = teleportTarget.rotation;
                if (controller != null) controller.enabled = true;

                isTeleported = true;
            }
            else
            {
                // ���̈ʒu�ɖ߂�
                if (controller != null) controller.enabled = false;
                player.position = originalPosition;
                player.rotation = originalRotation;
                if (controller != null) controller.enabled = true;

                isTeleported = false;
            }
        }
    }
}