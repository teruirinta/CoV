using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 3f;         // �ړ����x

    [Header("�J�����ݒ�")]
    public Transform cameraTransform;    // ��l�̎��_�J����
    public float lookSpeed = 1f;         // ���_�ړ����x�i�X�e�B�b�N�p�j
    public float mouseSensitivity = 2f;  // �}�E�X���_���x
    public float cameraPitchLimit = 80f; // �㉺���_�̐����p�x

    private CharacterController controller;
    private Vector3 velocity;            // �������x
    private float cameraPitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // �J�[�\���Œ聕��\��
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    void HandleMove()
    {
        // ���X�e�B�b�N�܂���WASD���͂��擾
        float horizontal = Input.GetAxis("Horizontal"); // A,D or ���X�e�B�b�NX
        float vertical = Input.GetAxis("Vertical");     // W,S or ���X�e�B�b�NY

        // �J�����̌�������ɑO�㍶�E���v�Z
        Vector3 move = (cameraTransform.forward * vertical + cameraTransform.right * horizontal);
        move.y = 0f;
        move.Normalize();

        // �ړ�����
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleLook()
    {
        // Xbox�E�X�e�B�b�N����
        float stickX = Input.GetAxis("RightStickHorizontal"); // �E�X�e�B�b�N��
        float stickY = Input.GetAxis("RightStickVertical");   // �E�X�e�B�b�N�c

        // �}�E�X����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ���_�ړ��i�X�e�B�b�N or �}�E�X�̍��v�j
        float lookX = stickX * lookSpeed + mouseX * mouseSensitivity;
        float lookY = stickY * lookSpeed + mouseY * mouseSensitivity;

        // �v���C���[�{�̂̐�����]
        transform.Rotate(Vector3.up * lookX);

        // �J�����̏㉺��]�i�s�b�`�j
        cameraPitch -= lookY;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
