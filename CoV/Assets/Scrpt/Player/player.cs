using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f;

    [Header("カメラ設定")]
    public Transform cameraTransform;
    public float lookSpeed = 2f;
    public float cameraPitchLimit = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;

    private bool isUpsideDown = false; // 上下反転フラグ

    [Header("ナイトスコープ時に消える壁")]
    public GameObject[] wallsToDisable;
    public Light cameraSpotlight;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleVisionInversion();
        HandleMove();
        HandleLook();
        HandleInteract();
        HandleWallVisibility();
        HandleSpotlight();
    }

    void HandleVisionInversion()
    {
        if (VisionManager.Instance == null) return;

        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldBeInverted != isUpsideDown)
        {
            isUpsideDown = shouldBeInverted;
            Debug.Log(isUpsideDown ? "🌀 上下反転モード ON" : "⬇ 上下反転モード OFF");

            // ✅ 重力反転を即座に反映（慣性リセット）
            velocity.y = 0f;

            // ✅ プレイヤーごと反転（Z軸180°回転）
            Vector3 euler = transform.eulerAngles;
            euler.z = isUpsideDown ? 180f : 0f;
            transform.eulerAngles = euler;
        }
    }

    void HandleMove()
    {
        float horizontal;
        float vertical;

        if (Input.GetJoystickNames().Length > 0)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        // カメラ基準で移動方向を決定
        Vector3 move = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        move.y = 0f;
        move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        // 重力処理（反転時は逆方向）
        bool isGrounded = controller.isGrounded;
        if (isGrounded && Mathf.Abs(velocity.y) < 0.1f)
            velocity.y = -2f;

        velocity.y += (isUpsideDown ? gravity : -gravity) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ✅ 反転時でもマウス操作方向は一定に保つ
        cameraPitch -= mouseY * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);

        transform.Rotate(Vector3.up * mouseX * lookSpeed);

        // カメラは上下のみ回転（Z軸180°はプレイヤーに適用されている）
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

    void HandleWallVisibility()
    {
        if (VisionManager.Instance == null || wallsToDisable == null) return;

        bool shouldDisable = (VisionManager.Instance.CurrentVision == VisionType.NightScope);

        foreach (GameObject wall in wallsToDisable)
        {
            if (wall != null)
            {
                // 見た目を消す
                Renderer renderer = wall.GetComponent<Renderer>();
                if (renderer != null) renderer.enabled = !shouldDisable;

                // 当たり判定を消す
                Collider collider = wall.GetComponent<Collider>();
                if (collider != null) collider.enabled = !shouldDisable;
            }
        }
    }

    void HandleSpotlight()
    {
        if (VisionManager.Instance == null || cameraSpotlight == null) return;

        bool shouldDisable = (VisionManager.Instance.CurrentVision == VisionType.NightScope);
        cameraSpotlight.enabled = !shouldDisable;
    }
}
