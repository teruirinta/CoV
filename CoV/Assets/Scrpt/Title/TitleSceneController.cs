using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleSceneController : MonoBehaviour
{
    [Header("次に遷移するシーン名")]
    public string nextSceneName = "AlphaScene";

    [Header("フェード用UI")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeSpeed = 1.5f;

    private bool isTransitioning = false;

    void Start()
    {
        // 最初は透明
        fadeCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (isTransitioning) return;

        // キーボード：スペースキー
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadeAndLoad());
        }

        // Xboxコントローラー：Aボタン
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        isTransitioning = true;

        // フェードアウト
        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}