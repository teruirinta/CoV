using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        // シングルトン化（シーンが変わっても残る）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 鍵を取得したデータを保存
    public void SaveKeyObtained(string keyId)
    {
        Debug.Log("[SaveManager] 鍵取得をセーブしました: " + keyId);
        PlayerPrefs.SetInt(KeyPref(keyId), 1);
        PlayerPrefs.Save();
    }

    // 鍵を持っているかを確認
    public bool IsKeySaved(string keyId)
    {
        return PlayerPrefs.GetInt(KeyPref(keyId), 0) == 1;
    }

    // 鍵を使ったときに削除
    public void ConsumeKey(string keyId)
    {
        PlayerPrefs.SetInt(KeyPref(keyId), 0);
        PlayerPrefs.Save();
    }

    // PlayerPrefs のキー名を作成
    private string KeyPref(string keyId) => $"HasKey_{keyId}";
}