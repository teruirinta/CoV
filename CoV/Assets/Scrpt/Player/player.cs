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

    [System.Serializable]
    public class HandIndicatorByTag
    {
        public string tag;
        public GameObject indicator;
    }

    [Header("インタラクト可能時の手表示")]
    public HandIndicatorByTag[] handIndicatorsByTag;

    [System.Serializable]
    public class TMPIndicatorByTag
    {
        public string tag;
        public GameObject indicator;
    }

    [Header("インタラクト可能時のTMP表示")]
    public TMPIndicatorByTag[] tmpIndicatorsByTag;

    [System.Serializable]
    public class PanelByTag
    {
        public string tag;
        public GameObject panel;
    }

    [Header("インタラクト可能時のパネル表示")]
    public PanelByTag[] panelsByTag;

    private BatteryItem currentBatteryItem;
    private float interactRange = 3f;

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
        HandleIndicators();
        HandleCameraCollision();
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        move.y = 0f;
        move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        bool isGrounded = controller.isGrounded;
        if (isGrounded && Mathf.Abs(velocity.y) < 0.1f)
            velocity.y = -2f;

        velocity.y += (isUpsideDown ? gravity : -gravity) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && move.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
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
            if (currentBatteryItem != null) return;

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

    void HandleSpotlight()
    {
        if (VisionManager.Instance == null || cameraSpotlight == null) return;

        bool shouldDisable = (VisionManager.Instance.CurrentVision == VisionType.NightScope);
        cameraSpotlight.enabled = !shouldDisable;
    }

    void HandleBatteryHighlight()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        BatteryItem hitBattery = null;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            hitBattery = hit.collider.GetComponent<BatteryItem>();
        }

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

    string DetectInteractTag()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.GetComponent<BatteryItem>() != null) return "Battery";
            if (hit.collider.GetComponent<OpenDoor>() != null) return "Door";
            if (hit.collider.CompareTag("TP")) return "TP";
            return hit.collider.tag;
        }

        return null;
    }

    void UpdateIndicators<T>(T[] indicators, string detectedTag) where T : class
    {
        foreach (var entry in indicators)
        {
            var tagProp = entry.GetType().GetField("tag");
            var objProp = entry.GetType().GetField("indicator") ?? entry.GetType().GetField("panel");

            if (tagProp != null && objProp != null)
            {
                string tag = tagProp.GetValue(entry) as string;
                GameObject go = objProp.GetValue(entry) as GameObject;

                if (go != null)
                    go.SetActive(tag == detectedTag);
            }
        }
    }

    void HandleIndicators()
    {
        string tag = DetectInteractTag();
        UpdateIndicators(handIndicatorsByTag, tag);
        UpdateIndicators(tmpIndicatorsByTag, tag);
        UpdateIndicators(panelsByTag, tag);
    }

    void HandleCameraCollision()
    {
        if (cameraTransform == null) return;

        Vector3 desiredPos = defaultCameraLocalPos;
        Vector3 worldPos = transform.TransformPoint(defaultCameraLocalPos);

        if (Physics.CheckSphere(worldPos, cameraCollisionRadius, wallMask))
        {
            desiredPos = defaultCameraLocalPos - new Vector3(0, 0, 0.05f);
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, desiredPos, Time.deltaTime * cameraAdjustSpeed);
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0 || footstepSource == null) return;

        if (!footstepSource.isPlaying)
        {
            int index = Random.Range(0, footstepClips.Length);
            footstepSource.pitch = Random.Range(0.9f, 1.2f);
            footstepSource.clip = footstepClips[index];
            footstepSource.Play();
        }
    }
}