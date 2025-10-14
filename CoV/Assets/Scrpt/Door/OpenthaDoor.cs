using UnityEngine;
using System.Collections;

public class OpenTheDoor : MonoBehaviour
{
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
    private Quaternion leftOpenedRotation;
    private Quaternion rightOpenedRotation;

    void Start()
    {
        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;

        leftOpenedRotation = Quaternion.Euler(leftDoor.localEulerAngles + new Vector3(0f, -openAngle, 0f));
        rightOpenedRotation = Quaternion.Euler(rightDoor.localEulerAngles + new Vector3(0f, openAngle, 0f));
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        if (!isAnimating)
        {
            StartCoroutine(RotateDoors());
        }
    }

    IEnumerator RotateDoors()
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
