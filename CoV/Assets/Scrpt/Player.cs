using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 3f;         // 移動速度

    [Header("カメラ設定")]
    public Transform cameraTransform;    // 一人称視点カメラ
    public float lookSpeed = 1f;         // 視点移動速度（スティック用）
    public float mouseSensitivity = 2f;  // マウス視点速度
    public float cameraPitchLimit = 80f; // 上下視点の制限角度

    private CharacterController controller;
    private Vector3 velocity;            // 落下速度
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
    }

    void HandleMove()
    {
        // 左スティックまたはWASD入力を取得
        float horizontal = Input.GetAxis("Horizontal"); // A,D or 左スティックX
        float vertical = Input.GetAxis("Vertical");     // W,S or 左スティックY

        // カメラの向きを基準に前後左右を計算
        Vector3 move = (cameraTransform.forward * vertical + cameraTransform.right * horizontal);
        move.y = 0f;
        move.Normalize();

        // 移動処理
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleLook()
    {
        // Xbox右スティック入力
        float stickX = Input.GetAxis("RightStickHorizontal"); // 右スティック横
        float stickY = Input.GetAxis("RightStickVertical");   // 右スティック縦

        // マウス入力
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 視点移動（スティック or マウスの合計）
        float lookX = stickX * lookSpeed + mouseX * mouseSensitivity;
        float lookY = stickY * lookSpeed + mouseY * mouseSensitivity;

        // プレイヤー本体の水平回転
        transform.Rotate(Vector3.up * lookX);

        // カメラの上下回転（ピッチ）
        cameraPitch -= lookY;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
