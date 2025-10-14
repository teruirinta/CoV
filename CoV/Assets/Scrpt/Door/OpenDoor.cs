using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour
{
    public float openAngle = -90f;        // 開く角度
    public float openDuration = 1.5f;      // 開くまでの時間（秒）
    private bool isOpen = false;
    private bool isAnimating = false;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openedRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f));
    }

    public void ToggleDoor()
    {
        if (!isAnimating)
        {
            StartCoroutine(RotateDoor());
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
