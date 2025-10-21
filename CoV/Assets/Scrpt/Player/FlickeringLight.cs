using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light flickerLight;           // 対象のライト
    public float minIntensity = 0.3f;    // 最小の明るさ
    public float maxIntensity = 1.0f;    // 最大の明るさ
    public float flickerSpeed = 0.1f;    // ちらつきの速さ（秒）

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= flickerSpeed)
        {
            float newIntensity = Random.Range(minIntensity, maxIntensity);
            flickerLight.intensity = newIntensity;
            timer = 0f;
        }
    }
}