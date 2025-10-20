using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        // シングルトン設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // プレイヤーが脱出に成功したとき
    public void OnPlayerEscaped()
    {
        Debug.Log("[GameController] プレイヤーが脱出しました！ステージクリア。");

        // ここに演出やシーン遷移処理を入れる
        // 例）SceneManager.LoadScene("NextStage");
    }

    // プレイヤーが敵に捕まったとき
    public void OnPlayerCaught()
    {
        Debug.Log("[GameController] プレイヤーが捕まりました。ゲームオーバー。");

        // ここにリトライやリザルト処理を入れる
    }
}