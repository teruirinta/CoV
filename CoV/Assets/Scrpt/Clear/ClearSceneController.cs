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

    [Header("クリアした階（1〜3）")]
    public int clearedFloor = 1; // ← ここでステージ番号を指定！

    private bool isTransitioning = false;

    void Start()
    {
        fadeCanvasGroup.alpha = 1f;
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            StartCoroutine(FadeOutAndLoad());
        }
    }

    IEnumerator FadeIn()
    {
        while (fadeCanvasGroup.alpha > 0f)
        {
            fadeCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutAndLoad()
    {
        isTransitioning = true;

        // ✅ ここでクリア情報を記録！
        switch (clearedFloor)
        {
            case 1:
                GameProgress.floor1Cleared = true;
                break;
            case 2:
                GameProgress.floor2Cleared = true;
                break;
            case 3:
                GameProgress.floor3Cleared = true;
                break;
        }

        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
