using UnityEngine;

public class FadeArrow : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float speed = 2.0f;
    public float minAlpha = 0.3f;
    public float maxAlpha = 1.0f;

    void Awake()
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