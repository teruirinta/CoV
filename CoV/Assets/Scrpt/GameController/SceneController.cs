using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    /// <summary>
    /// 現在のシーン名を保存する（ゲームオーバー前などに呼び出す）
    /// </summary>
    public static void CurrentSceneName()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("PreviousScene", currentScene);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 保存された前のシーンに戻る（やり直し時に呼び出す）
    /// </summary>
    public static void ReturnToPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");

        if (!string.IsNullOrEmpty(previousScene) && Application.CanStreamedLevelBeLoaded(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("前のシーンが見つからないか、ビルド設定に含まれていません！");
        }
    }
}
