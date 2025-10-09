using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour

{

    [Header("移動設定")]
    public float moveSpeed = 3f;          // 移動速度
    public float gravity = -9.81f;        // 重力加速度

    [Header("カメラ設定")]
    public Transform cameraTransform;     // 一人称視点カメラ
    public float lookSpeed = 1f;          // スティック視点速度
    public float mouseSensitivity = 2f;   // マウス感度
    public float cameraPitchLimit = 80f;  // 上下視点の制限角度

    private CharacterController controller;
    private Vector3 velocity;             // 重力による速度
    private float cameraPitch = 0f;

    void Start()

    {
        controller = GetComponent<CharacterController>();

        // カーソル固定＆非表示
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()

    {
        HandleLook();
        HandleMove();
        HandleInteract();

    }

    void HandleMove()

    {
        float horizontal;
        float vertical;

        // 🎮 コントローラー接続判定（接続中なら滑らか入力）
        if (Input.GetJoystickNames().Length > 0)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
        else
        {
            // ⌨️ キーボード操作時はピタッと止まる
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        // カメラの向きを基準に前後左右を計算
        Vector3 move = (cameraTransform.forward * vertical + cameraTransform.right * horizontal);
        move.y = 0f;
        move.Normalize();

        // 移動処理
        controller.Move(move * moveSpeed * Time.deltaTime);

        // === 重力処理 ===
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に押し付けるような軽い値
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    void HandleLook()

    {
        // 🎮 右スティック入力
        float stickX = Input.GetAxis("RightStickHorizontal");
        float stickY = Input.GetAxis("RightStickVertical");

        // 🖱️ マウス入力
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 合成（スティック＋マウス）
        float lookX = stickX * lookSpeed + mouseX * mouseSensitivity;
        float lookY = stickY * lookSpeed + mouseY * mouseSensitivity;

        // プレイヤー本体の水平回転
        transform.Rotate(Vector3.up * lookX);

        // カメラの上下回転（ピッチ）
        cameraPitch -= lookY;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

    }
    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f)) // 3m以内
            {
                OpenDoor door = hit.collider.GetComponent<OpenDoor>();
                if (door != null)
                {
                    door.ToggleDoor();
                }
            }
        }
    }

}

