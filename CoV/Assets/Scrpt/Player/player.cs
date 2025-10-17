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
    private bool isUpsideDown = false;

    [Header("ナイトスコープ時に表示する壁")]
    public GameObject[] wallsToEnableInNightScope;
    public Light cameraSpotlight;

    [Header("ナイトスコープ時に非表示にする壁")]
    public GameObject[] wallsToDisableInNightScope;

    [Header("インタラクト可能時の手表示")]
    public GameObject handIndicator;

    // 🔋 追加: バッテリー検出用
    private BatteryItem currentBatteryItem;
    private float interactRange = 3f;

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
        HandleBatteryHighlight();
        HandleHandIndicator();
    }

    void HandleVisionInversion()
    {
        if (VisionManager.Instance == null) return;

        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldBeInverted != isUpsideDown)
        {
            isUpsideDown = shouldBeInverted;
            Debug.Log(isUpsideDown ? "🌀 上下反転モード ON" : "⬇ 上下反転モード OFF");

            velocity.y = 0f;
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

        Vector3 move = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        move.y = 0f;
        move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

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

        cameraPitch -= mouseY * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);
        transform.Rotate(Vector3.up * mouseX * lookSpeed);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (currentBatteryItem != null)
            {
                // ✅ BatteryItemが自分の内部で処理するのでここでは何もしない
                return;
            }

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f))
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
        if (VisionManager.Instance == null) return;

        bool isNightScope = (VisionManager.Instance.CurrentVision == VisionType.NightScope);

        // ナイトスコープ時に非表示にする壁
        if (wallsToDisableInNightScope != null)
        {
            foreach (GameObject wall in wallsToDisableInNightScope)
            {
                if (wall != null)
                {
                    Renderer renderer = wall.GetComponent<Renderer>();
                    if (renderer != null) renderer.enabled = !isNightScope;

                    Collider collider = wall.GetComponent<Collider>();
                    if (collider != null) collider.enabled = !isNightScope;
                }
            }
        }

        // ナイトスコープ時に表示する壁
        if (wallsToEnableInNightScope != null)
        {
            foreach (GameObject wall in wallsToEnableInNightScope)
            {
                if (wall != null)
                {
                    Renderer renderer = wall.GetComponent<Renderer>();
                    if (renderer != null) renderer.enabled = isNightScope;

                    Collider collider = wall.GetComponent<Collider>();
                    if (collider != null) collider.enabled = isNightScope;
                }
            }
        }
    }

    void HandleSpotlight()
    {
        if (VisionManager.Instance == null || cameraSpotlight == null) return;
        bool shouldDisable = (VisionManager.Instance.CurrentVision == VisionType.NightScope);
        cameraSpotlight.enabled = !shouldDisable;
    }

    // 🔋 バッテリーのアウトラインを制御する処理
    void HandleBatteryHighlight()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        BatteryItem hitBattery = null;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            hitBattery = hit.collider.GetComponent<BatteryItem>();
        }

        // 前に見ていたバッテリーと異なるなら、アウトラインを切り替える
        if (currentBatteryItem != hitBattery)
        {
            if (currentBatteryItem != null)
            {
                QuickOutline outline = currentBatteryItem.GetComponent<QuickOutline>();
                if (outline != null) outline.enabled = false;
            }

            currentBatteryItem = hitBattery;

            if (currentBatteryItem != null)
            {
                QuickOutline outline = currentBatteryItem.GetComponent<QuickOutline>();
                if (outline != null) outline.enabled = true;
            }
        }
    }
    void HandleHandIndicator()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        bool canInteract = false;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.GetComponent<BatteryItem>() != null || hit.collider.GetComponent<OpenDoor>() != null
                || hit.collider.CompareTag("TP"))
            {
                canInteract = true;
            }
        }

        if (handIndicator != null)
        {
            handIndicator.SetActive(canInteract);
        }
    }
}
