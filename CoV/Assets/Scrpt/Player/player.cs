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

    [Header("ナイトスコープ時に表示する壁")]
    public GameObject[] wallsToEnableInNightScope;
    [Header("ナイトスコープ時に非表示にする壁")]
    public GameObject[] wallsToDisableInNightScope;
    public Light cameraSpotlight;

    private BatteryItem currentBatteryItem;
    private float interactRange = 3f;

    [System.Serializable]
    public class TMPIndicatorByTag
    {
        public string tag;
        public GameObject indicator;
    }

    [Header("インタラクト可能時の表示")]
    public TMPIndicatorByTag[] tmpIndicatorsByTag;

    [Header("記憶メガネ時に表示するオブジェクト")]
    public GameObject[] wallsToEnableMemoryVision;
    [Header("記憶メガネ時に非表示にするオブジェクト")]
    public GameObject[] wallsToDisableMemoryVision;

    public GameObject interactUIKeyboard;

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
        HandleMemoryVisionVisibility();
        HandleSpotlight();
        HandleBatteryHighlight();
        HandleIndicators();
        HandleCameraCollision();
        HandleEnemyCollision();

        UpdateInteractUI(RaycastInteractable() != null);
    }

    // --- 子オブジェクト含めて表示切替 ---
    void SetObjectsVisibilityRecursive(GameObject[] groups, bool visible)
    {
        foreach (GameObject rootObj in groups)
        {
            if (rootObj == null) continue;

            Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers) r.enabled = visible;

            Collider[] colliders = rootObj.GetComponentsInChildren<Collider>(true);
            foreach (Collider c in colliders) c.enabled = visible;
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

    void HandleLook()
    {
        float lookX = Input.GetAxis("Mouse X");
        float lookY = Input.GetAxis("Mouse Y");

        cameraPitch = Mathf.Clamp(cameraPitch - lookY * lookSpeed, -cameraPitchLimit, cameraPitchLimit);
        transform.Rotate(Vector3.up * lookX * lookSpeed);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject target = RaycastInteractable();
            if (target == null) return;

            var door = target.GetComponent<OpenDoor>();
            if (door != null)
            {
                door.ToggleDoor(transform);
                return;
            }

            var battery = target.GetComponent<BatteryItem>();
            if (battery != null)
            {
                Destroy(battery.gameObject);
                return;
            }
        }
    }

    void HandleBatteryHighlight()
    {
        GameObject target = RaycastInteractable();
        BatteryItem hitBattery = target != null ? target.GetComponent<BatteryItem>() : null;

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

    void HandleIndicators()
    {
        string tag = DetectInteractTag();
        UpdateIndicators(tmpIndicatorsByTag, tag);
    }

    string DetectInteractTag()
    {
        GameObject target = RaycastInteractable();
        if (target == null) return null;

        if (target.GetComponent<BatteryItem>() != null) return "Battery";
        if (target.GetComponent<OpenDoor>() != null) return "Door";
        if (target.CompareTag("TP")) return "TP";
        if (target.CompareTag("Key")) return "Key";
        if (target.CompareTag("Salt")) return "Salt";
        if (target.CompareTag("Goal")) return "Goal";
        if (target.CompareTag("Battery")) return "Battery";

        return target.tag;
    }

    void UpdateIndicators<T>(T[] indicators, string detectedTag) where T : class
    {
        foreach (var entry in indicators)
        {
            var tagProp = entry.GetType().GetField("tag");
            var objProp = entry.GetType().GetField("indicator");
            if (tagProp == null || objProp == null) continue;

            string tag = tagProp.GetValue(entry) as string;
            GameObject go = objProp.GetValue(entry) as GameObject;
            if (go != null) go.SetActive(tag == detectedTag);
        }
    }

    GameObject RaycastInteractable()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.CompareTag("Door") ||
                obj.CompareTag("Battery") ||
                obj.CompareTag("Key") ||
                obj.CompareTag("Salt") ||
                obj.CompareTag("Goal") ||
                obj.CompareTag("TP"))
            {
                return obj;
            }
        }
        return null;
    }

    void HandleWallVisibility()
    {
        if (VisionManager.Instance == null) return;
        bool isNightScope = (VisionManager.Instance.CurrentVision == VisionType.NightScope);

        SetObjectsVisibilityRecursive(wallsToDisableInNightScope, !isNightScope);
        SetObjectsVisibilityRecursive(wallsToEnableInNightScope, isNightScope);
    }

    void HandleMemoryVisionVisibility()
    {
        if (VisionManager.Instance == null) return;
        bool isMemory = (VisionManager.Instance.CurrentVision == VisionType.MemoryVision);

        SetObjectsVisibilityRecursive(wallsToDisableMemoryVision, !isMemory);
        SetObjectsVisibilityRecursive(wallsToEnableMemoryVision, isMemory);
    }

    void HandleSpotlight()
    {
        if (VisionManager.Instance == null || cameraSpotlight == null) return;
        cameraSpotlight.enabled = (VisionManager.Instance.CurrentVision != VisionType.NightScope);
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
                SceneController.CurrentSceneName();
                SceneManager.LoadScene("GameOver");
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
                HandleMemoryVisionVisibility();
                HandleSpotlight();
            });
        }
    }

    void UpdateInteractUI(bool canInteract)
    {
        interactUIKeyboard.SetActive(canInteract);
    }
}
