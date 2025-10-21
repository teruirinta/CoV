using UnityEngine;

public class CameraM : MonoBehaviour
{
    public float bobSpeed = 2.0f;           // 揺れのテンポ
    public float bobAmount = 0.01f;         // 歩行揺れ（小さめ）
    public float noiseAmount = 0.005f;      // ノイズ（微細）
    public float rollAmount = 1.0f;         // 頭の傾き（控えめ）
    public CharacterController controller;

    private Vector3 startPos;
    private float timer = 0f;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        timer += Time.deltaTime * bobSpeed;

        if (controller.isGrounded && controller.velocity.magnitude > 0.01f)
        {
            float bobX = Mathf.Cos(timer * 0.5f) * bobAmount;
            float bobY = Mathf.Sin(timer) * bobAmount;

            float noiseX = (Mathf.PerlinNoise(Time.time * 1.2f, 0f) - 0.5f) * noiseAmount;
            float noiseY = (Mathf.PerlinNoise(0f, Time.time * 1.5f) - 0.5f) * noiseAmount;

            float rollZ = Mathf.Sin(timer * 0.7f) * rollAmount;
            transform.localRotation = Quaternion.Euler(0, 0, rollZ);

            transform.localPosition = startPos + new Vector3(bobX + noiseX, bobY + noiseY, 0);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * bobSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * bobSpeed);
        }
    }
}
