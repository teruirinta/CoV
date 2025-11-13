using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    // フェードアウト→処理→フェードイン
    public void FadeOutIn(System.Action onMidFade = null)
    {
        StartCoroutine(FadeRoutine(onMidFade));
    }

    private IEnumerator FadeRoutine(System.Action onMidFade)
    {
        yield return StartCoroutine(FadeAlpha(0f, 1f));
        onMidFade?.Invoke();
        yield return StartCoroutine(FadeAlpha(1f, 0f));
    }

    // 🌟 即座に真っ暗→処理→ゆっくりフェードイン
    public void FadeInstantOutThenIn(System.Action onMidFade = null)
    {
        // すぐに真っ暗にする
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        onMidFade?.Invoke();
        StartCoroutine(FadeAlpha(1f, 0f)); // ゆっくり明るく
    }

    private IEnumerator FadeAlpha(float from, float to)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, to);
    }

    // テスト用：Fキーでフェード確認
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 通常のフェード
            // FadeOutIn(() => Debug.Log("フェード中に何かするよ！"));

            //  即座に暗転 → 明るく
            FadeInstantOutThenIn(() => Debug.Log("真っ暗の中で処理！"));
        }
    }
}
