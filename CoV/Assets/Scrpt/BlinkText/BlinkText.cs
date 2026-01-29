using UnityEngine;

public class BlinkText : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float speed = 1.5f;   // フェード速度
    public float minAlpha = 0.2f;
    public float maxAlpha = 1.0f;

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * speed, maxAlpha - minAlpha) + minAlpha;
        canvasGroup.alpha = alpha;
    }
}