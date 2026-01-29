using UnityEngine;
using UnityEngine.SceneManagement;

public class StageInitializer : MonoBehaviour
{
    void Start()
    {
        string stageName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastStageName", stageName);
        PlayerPrefs.Save();
    }
}
