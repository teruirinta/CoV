using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour
{
    [Header("ドア設定")]
    public float openAngle = -90f;        // 開く角度（正の値でOK、方向は自動判定）
    public float openDuration = 1.5f;    // 開くまでの時間（秒）

    private bool isOpen = false;
    private bool isAnimating = false;

    private Quaternion closedRotation;   // 初期角度
    private Quaternion openedRotation;   // 開いた角度（毎回プレイヤー位置で決まる）

    [Header("サウンド設定")]
    public AudioSource doorAudioSource;
    public AudioClip openSound;

    void Start()
    {
        closedRotation = transform.rotation;
    }

    /// <summary>
    /// プレイヤーを引数に取るバージョンのToggleDoor
    /// </summary>
    public void ToggleDoor(Transform playerTransform)
    {
        if (isAnimating) return;

        // プレイヤーの位置からドアの「どちら側」にいるかを判定
        Vector3 toPlayer = playerTransform.position - transform.position;
        float dot = Vector3.Dot(transform.forward, toPlayer.normalized);

        // プレイヤーがドアの前面にいるなら正方向、背面なら逆方向に開く
        float direction = (dot > 0) ? 1f : -1f;

        // 回転の最終角度を算出
        openedRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle * direction, 0f));

        StartCoroutine(RotateDoor());

        if (doorAudioSource != null && openSound != null)
        {
            doorAudioSource.clip = openSound;
            doorAudioSource.Play();
        }
    }

    IEnumerator RotateDoor()
    {
        isAnimating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = isOpen ? closedRotation : openedRotation;

        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / openDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isOpen = !isOpen;
        isAnimating = false;
    }
}
