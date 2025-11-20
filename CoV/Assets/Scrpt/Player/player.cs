using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(CharacterController))]
public class player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isUpsideDown = false;

    // === カメラ関連 ===
    [Header("カメラ設定")]
    public Transform cameraTransform;
    public float lookSpeed = 2f;
    public float cameraPitchLimit = 80f;
    private float cameraPitch = 0f;
    private VisionType previousVision;
    private Vector3 defaultCameraLocalPos;

    [Header("カメラ衝突設定")]
    public LayerMask wallMask;
    public float cameraCollisionRadius = 0.2f;
    public float cameraAdjustSpeed = 10f;

    // === ナイトスコープ関連 ===
    [Header("ナイトスコープ時に表示する壁")]
    public GameObject[] wallsToEnableInNightScope;
    [Header("ナイトスコープ時に非表示にする壁")]
    public GameObject[] wallsToDisableInNightScope;
    public Light cameraSpotlight;

    // === インタラクト関連 ===
    private BatteryItem currentBatteryItem;
    private float interactRange = 3f;

    [System.Serializable]
    public class HandIndicatorByTag
    {
        public string tag;
        public GameObject indicator;
    }

    [System.Serializable]
    public class TMPIndicatorByTag
    {
        public string tag;
        public GameObject indicator;
    }

    [System.Serializable]
    public class PanelByTag
    {
        public string tag;
        public GameObject panel;
    }

    [Header("インタラクト可能時の表示")]
    public HandIndicatorByTag[] handIndicatorsByTag;
    public TMPIndicatorByTag[] tmpIndicatorsByTag;
    public PanelByTag[] panelsByTag;



    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
            defaultCameraLocalPos = cameraTransform.localPosition;

        if (VisionManager.Instance != null)
            previousVision = VisionManager.Instance.CurrentVision;
    }


    void Update()
    {
        HandleVisionChangeWithFade();
        HandleMove();
        HandleLook();
        HandleInteract();
        HandleWallVisibility();
        HandleSpotlight();
        HandleBatteryHighlight();
        HandleIndicators();
        HandleCameraCollision();
        HandleEnemyCollision();
    }


    void HandleVisionInversion()
    {
        if (VisionManager.Instance == null) return;

        bool shouldBeInverted = (VisionManager.Instance.CurrentVision == VisionType.Inverted);

        if (shouldBeInverted != isUpsideDown)
        {
            isUpsideDown = shouldBeInverted;
            velocity.y = 0f;

            // フェード付きで視点切り替え
            FindObjectOfType<Fade>().FadeOutIn(() =>
            {
                Vector3 euler = transform.eulerAngles;
                euler.z = isUpsideDown ? 180f : 0f;
                transform.eulerAngles = euler;
            });
        }
    }



    void HandleMove()
    {
        if (controller == null || !controller.enabled) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

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


    // 🎮 Xbox右スティック & マウス両対応
    void HandleLook()
    {
        float lookX = 0f;
        float lookY = 0f;

        // マウス入力
        lookX += Input.GetAxis("Mouse X");
        lookY += Input.GetAxis("Mouse Y");

        // Xbox右スティック（Input Manager 設定で追加した軸）
        lookX += Input.GetAxis("RightStickX");
        lookY -= Input.GetAxis("RightStickY");

        cameraPitch = Mathf.Clamp(cameraPitch - lookY * lookSpeed, -cameraPitchLimit, cameraPitchLimit);
        transform.Rotate(Vector3.up * lookX * lookSpeed);
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
                    door.ToggleDoor(transform);
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
                var renderer = wall.GetComponent<Renderer>();
                if (renderer != null) renderer.enabled = !isNightScope;

                var collider = wall.GetComponent<Collider>();
                if (collider != null) collider.enabled = !isNightScope;
            }
        }

        foreach (GameObject wall in wallsToEnableInNightScope)
        {
            if (wall != null)
            {
                var renderer = wall.GetComponent<Renderer>();
                if (renderer != null) renderer.enabled = isNightScope;

                var collider = wall.GetComponent<Collider>();
                if (collider != null) collider.enabled = isNightScope;
            }
        }
    }

    void HandleSpotlight()
    {
        if (VisionManager.Instance == null || cameraSpotlight == null) return;
        cameraSpotlight.enabled = (VisionManager.Instance.CurrentVision != VisionType.NightScope);
    }

    void HandleBatteryHighlight()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        BatteryItem hitBattery = null;

        if (Physics.Raycast(ray, out hit, interactRange))
            hitBattery = hit.collider.GetComponent<BatteryItem>();

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
            if (tagProp == null || objProp == null) continue;

            string tag = tagProp.GetValue(entry) as string;
            GameObject go = objProp.GetValue(entry) as GameObject;
            if (go != null) go.SetActive(tag == detectedTag);
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
            desiredPos = defaultCameraLocalPos - new Vector3(0, 0, 0.05f);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, desiredPos, Time.deltaTime * cameraAdjustSpeed);
    }



    void HandleEnemyCollision()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                SceneController.CurrentSceneName(); // 今のシーン名を記録！
                SceneManager.LoadScene("GameOver"); // ゲームオーバー画面へ
            }
        }
    }


    void HandleVisionChangeWithFade()
    {
        if (VisionManager.Instance == null) return;

        VisionType currentVision = VisionManager.Instance.CurrentVision;

        if (currentVision != previousVision)
        {
            previousVision = currentVision;

            FindObjectOfType<Fade>().FadeInstantOutThenIn(() =>
            {
                isUpsideDown = (currentVision == VisionType.Inverted);
                velocity.y = 0f;
                Vector3 euler = transform.eulerAngles;
                euler.z = isUpsideDown ? 180f : 0f;
                transform.eulerAngles = euler;

                HandleWallVisibility();
                HandleSpotlight();
            });
        }
    }

}
