using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClearSceneController : MonoBehaviour
{
    [Header("次に遷移するシーン名")]
    public string nextSceneName = "TitleScene";

    [Header("フェード用UI")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeSpeed = 1.5f;

    private bool isTransitioning = false;

    void Start()
    {
        // 最初は真っ黒（フェードイン開始）
        fadeCanvasGroup.alpha = 1f;
        StartCoroutine(FadeIn());
    }


    void OnStageClear()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("ClearedStage", currentScene);
        PlayerPrefs.Save();

        SceneManager.LoadScene("EscapeResult");
    }


    void Update()
    {
        if (isTransitioning) return;

        // キーボード：スペースキー
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadeOutAndLoad());
        }

        // Xboxコントローラー：Aボタン
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            StartCoroutine(FadeOutAndLoad());
        }
    }

    // ✅ フェードイン（暗 → 明）
    IEnumerator FadeIn()
    {
        while (fadeCanvasGroup.alpha > 0f)
        {
            fadeCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    // ✅ フェードアウト → 次のシーンへ
    IEnumerator FadeOutAndLoad()
    {
        isTransitioning = true;

        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

   

}
