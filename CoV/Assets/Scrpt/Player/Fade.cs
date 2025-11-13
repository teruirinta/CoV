using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Fade : MonoBehaviour
{
    public RectTransform topPanel;
    public RectTransform bottomPanel;
    public float speed = 500f;
    public float delay = 0.5f;

    private Vector2 topStartPos;
    private Vector2 bottomStartPos;
    private Vector2 topClosedPos;
    private Vector2 bottomClosedPos;

    void Start()
    {
        topStartPos = topPanel.anchoredPosition;
        bottomStartPos = bottomPanel.anchoredPosition;
        topClosedPos = new Vector2(topStartPos.x, 0);
        bottomClosedPos = new Vector2(bottomStartPos.x, 0);

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        // •Â‚¶‚é
        while (topPanel.anchoredPosition.y > topClosedPos.y)
        {
            topPanel.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
            bottomPanel.anchoredPosition += new Vector2(0, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        // ŠJ‚­
        while (topPanel.anchoredPosition.y < topStartPos.y)
        {
            topPanel.anchoredPosition += new Vector2(0, speed * Time.deltaTime);
            bottomPanel.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
            yield return null;
        }
    }
}