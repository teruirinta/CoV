using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static string sceneName;

    public static void CurrentSceneName()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    public static void BackToBeforeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("戻るシーンが記録されていません！");
        }
    }
}
