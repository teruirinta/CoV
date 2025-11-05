using UnityEngine;
using UnityEngine.InputSystem; // ← 新Input System対応

[RequireComponent(typeof(CharacterController))]
public class player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f;

    [Header("カメラ設定")]
    public Transform cameraTransform;
    public float lookSpeed = 100f;
    public float cameraPitchLimit = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private bool isUpsideDown = false;

    [Header("ナイトスコープ設定")]
    public GameObject[] wallsToEnableInNightScope;
    public GameObject[] wallsToDisableInNightScope;
    public Light cameraSpotlight;

    [Header("カメラ衝突設定")]
    public LayerMask wallMask;
    public float cameraCollisionRadius = 0.2f;
    public float cameraAdjustSpeed = 10f;
    private Vector3 defaultCameraLocalPos;

    [Header("足音設定")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.8f;
    private float footstepTimer = 0f;

    private BatteryItem currentBatteryItem;
    private float interactRange = 3f;

    // 新Input System入力値保持
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool interactPressed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
            defaultCameraLocalPos = cameraTransform.localPosition;
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
        HandleCameraCollision();
    }

    // ===== 新Input System入力イベント =====
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            interactPressed = true;
    }

    // ======== 視点反転処理 ========
    void HandleVisionInversion()
    {
        if (VisionManager.Instance == null) return;
        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldBeInverted != isUpsideDown)
        {
            isUpsideDown = shouldBeInverted;
            velocity.y = 0f;
            Vector3 euler = transform.eulerAngles;
            euler.z = isUpsideDown ? 180f : 0f;
            transform.eulerAngles = euler;
        }
    }

    // ======== 移動処理（左スティック + WASD対応）========
    void HandleMove()
    {
        Vector3 move = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        move.y = 0f;
        move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        bool isGrounded = controller.isGrounded;
        if (isGrounded && Mathf.Abs(velocity.y) < 0.1f)
            velocity.y = -2f;

        velocity.y += (isUpsideDown ? gravity : -gravity) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 足音
        if (isGrounded && move.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else footstepTimer = 0f;
    }

    // ======== カメラ回転処理（マウス & 右スティック）========
    void HandleLook()
    {
        float lookX = lookInput.x * lookSpeed * Time.deltaTime;
        float lookY = lookInput.y * lookSpeed * Time.deltaTime;

        cameraPitch = Mathf.Clamp(cameraPitch - lookY, -cameraPitchLimit, cameraPitchLimit);
        transform.Rotate(Vector3.up * lookX);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    // ======== インタラクト処理（Eキー or Aボタン）========
    void HandleInteract()
    {
        if (!interactPressed) return;
        interactPressed = false;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            var door = hit.collider.GetComponent<OpenDoor>();
            if (door != null)
            {
                door.ToggleDoor();
            }
        }
    }

    // ======== 壁ON/OFF ========
    void HandleWallVisibility()
    {
        if (VisionManager.Instance == null) return;

        bool isNightScope = (VisionManager.Instance.CurrentVision == VisionType.NightScope);

        foreach (GameObject wall in wallsToDisableInNightScope)
        {
            if (wall)
            {
                var renderer = wall.GetComponent<Renderer>();
                if (renderer) renderer.enabled = !isNightScope;
                var collider = wall.GetComponent<Collider>();
                if (collider) collider.enabled = !isNightScope;
            }
        }

        foreach (GameObject wall in wallsToEnableInNightScope)
        {
            if (wall)
            {
                var renderer = wall.GetComponent<Renderer>();
                if (renderer) renderer.enabled = isNightScope;
                var collider = wall.GetComponent<Collider>();
                if (collider) collider.enabled = isNightScope;
            }
        }
    }

    // ======== スポットライト制御 ========
    void HandleSpotlight()
    {
        if (VisionManager.Instance == null || cameraSpotlight == null) return;
        bool shouldDisable = (VisionManager.Instance.CurrentVision == VisionType.NightScope);
        cameraSpotlight.enabled = !shouldDisable;
    }

    // ======== バッテリーアウトライン ========
    void HandleBatteryHighlight()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            BatteryItem battery = hit.collider.GetComponent<BatteryItem>();
            if (currentBatteryItem != battery)
            {
                if (currentBatteryItem)
                {
                    QuickOutline o = currentBatteryItem.GetComponent<QuickOutline>();
                    if (o) o.enabled = false;
                }
                currentBatteryItem = battery;
                if (currentBatteryItem)
                {
                    QuickOutline o = currentBatteryItem.GetComponent<QuickOutline>();
                    if (o) o.enabled = true;
                }
            }
        }
        else if (currentBatteryItem)
        {
            QuickOutline o = currentBatteryItem.GetComponent<QuickOutline>();
            if (o) o.enabled = false;
            currentBatteryItem = null;
        }
    }

    // ======== カメラ衝突補正 ========
    void HandleCameraCollision()
    {
        if (cameraTransform == null) return;

        Vector3 desiredPos = defaultCameraLocalPos;
        Vector3 worldPos = transform.TransformPoint(defaultCameraLocalPos);

        if (Physics.CheckSphere(worldPos, cameraCollisionRadius, wallMask))
            desiredPos = defaultCameraLocalPos - new Vector3(0, 0, 0.05f);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, desiredPos, Time.deltaTime * cameraAdjustSpeed);
    }

    void PlayFootstep()
    {
        if (footstepSource == null || footstepClips.Length == 0) return;
        int index = Random.Range(0, footstepClips.Length);
        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.clip = footstepClips[index];
        footstepSource.Play();
    }
}
