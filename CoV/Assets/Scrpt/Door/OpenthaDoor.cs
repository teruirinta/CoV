using UnityEngine;
using System.Collections;

public class OpenTheDoor : MonoBehaviour
{
    [Header("ドア設定")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform player;              // プレイヤーのTransformをInspectorで設定
    public float openAngle = 90f;
    public float openDuration = 1.5f;
    public float activationDistance = 3f; // プレイヤーが近づく距離

    private bool isOpen = false;
    private bool isAnimating = false;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;

    [Header("サウンド設定")]
    public AudioSource doorAudioSource;
    public AudioClip openSound;

    void Start()
    {
        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // ★ 修正：距離条件を括弧でまとめてバグ防止
        if (distance <= activationDistance &&
            (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Jump")))
        {
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        if (isAnimating) return;

        // プレイヤーの位置からドアのどちら側にいるかを判定
        Vector3 toPlayer = player.position - transform.position;
        float dot = Vector3.Dot(transform.forward, toPlayer.normalized);

        // プレイヤーが前面にいれば奥向き（＋）、背面なら手前向き（−）
        float direction = (dot > 0) ? 1f : -1f;

        // ★ 修正：初期角度を基準に開く角度を作る（ズレ防止）
        Quaternion leftOpenedRotation =
            leftClosedRotation * Quaternion.Euler(0f, -openAngle * direction, 0f);

        Quaternion rightOpenedRotation =
            rightClosedRotation * Quaternion.Euler(0f, openAngle * direction, 0f);

        StartCoroutine(RotateDoors(leftOpenedRotation, rightOpenedRotation));

        if (doorAudioSource != null && openSound != null)
        {
            doorAudioSource.clip = openSound;
            doorAudioSource.Play();
        }
    }

    IEnumerator RotateDoors(Quaternion leftOpenedRotation, Quaternion rightOpenedRotation)
    {
        isAnimating = true;

        Quaternion leftStart = leftDoor.localRotation;
        Quaternion rightStart = rightDoor.localRotation;

        Quaternion leftEnd = isOpen ? leftClosedRotation : leftOpenedRotation;
        Quaternion rightEnd = isOpen ? rightClosedRotation : rightOpenedRotation;

        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            float t = elapsed / openDuration;
            leftDoor.localRotation = Quaternion.Slerp(leftStart, leftEnd, t);
            rightDoor.localRotation = Quaternion.Slerp(rightStart, rightEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        leftDoor.localRotation = leftEnd;
        rightDoor.localRotation = rightEnd;

        isOpen = !isOpen;
        isAnimating = false;
    }
}
