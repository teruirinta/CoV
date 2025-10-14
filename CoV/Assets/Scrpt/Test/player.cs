using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;      // 移動速度
    public float gravity = 9.81f;     // 重力加速度

    [Header("カメラ設定")]
    public Transform cameraTransform; // 一人称視点カメラ
    public float lookSpeed = 2f;      // 視点速度
    public float cameraPitchLimit = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;

    private bool isUpsideDown = false; // 上下反転状態
    private Quaternion normalRotation;
    private Quaternion invertedRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 通常時と上下反転時の回転を記憶
        normalRotation = transform.rotation;
        invertedRotation = Quaternion.Euler(180f, transform.eulerAngles.y, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleVisionInversion(); // 視界状態に応じて上下反転切り替え
        HandleMove();
        HandleLook();
    }

    void HandleVisionInversion()
    {
        // VisionManagerが存在しなければ何もしない
        if (VisionManager.Instance == null) return;

        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldBeInverted != isUpsideDown)
        {
            isUpsideDown = shouldBeInverted;

            if (isUpsideDown)
                Debug.Log("🌀 上下反転モード ON");
            else
                Debug.Log("⬇ 上下反転モード OFF");
        }

        // プレイヤー全体をスムーズに180°反転
        Quaternion targetRot = isUpsideDown ? invertedRotation : normalRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 5f);

        // カメラもZ軸方向に180°反転
        float targetZ = isUpsideDown ? 180f : 0f;
        Vector3 currentEuler = cameraTransform.localEulerAngles;
        float newZ = Mathf.LerpAngle(currentEuler.z, targetZ, Time.deltaTime * 5f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, newZ);
    }

    void HandleMove()
    {
        float horizontal;
        float vertical;

        // 🎮 コントローラー対応
        if (Input.GetJoystickNames().Length > 0)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
        else
        {
            // ⌨️ キーボード入力（ピタッと止まる）
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        // カメラ基準の方向
        Vector3 move = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        move.y = 0f;
        move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        // === 重力処理 ===
        bool isGrounded = controller.isGrounded;

        if (isGrounded && Mathf.Abs(velocity.y) < 0.1f)
            velocity.y = -2f; // 地面に押し付ける軽い値

        // 上下反転時は重力反転
        velocity.y += (isUpsideDown ? gravity : -gravity) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        cameraPitch -= mouseY * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);

        // プレイヤーの水平回転
        transform.Rotate(Vector3.up * mouseX * lookSpeed);

        // カメラの上下回転（Z反転は別処理で対応）
        Vector3 currentEuler = cameraTransform.localEulerAngles;
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, currentEuler.z);
    }
}
