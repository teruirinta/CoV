using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("Tキーを押した時に戻るシーンの名前")]
    public string titleSceneName = "Title";

    void Update()
    {
        // Rキー：直前のゲームプレイシーン（記録されたシーン）にリトライ
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneController.BackToBeforeScene();
        }

        // Tキー：タイトル画面へ戻る
        if (Input.GetKeyDown(KeyCode.T))
        {
            // インスペクターで指定したシーン名に遷移
            SceneManager.LoadScene(titleSceneName);
        }
    }
}