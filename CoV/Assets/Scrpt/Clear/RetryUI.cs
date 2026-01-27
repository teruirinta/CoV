using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RetryUI : MonoBehaviour
{
    [Header("UI要素")]
    public Text[] optionTexts; // [0] = ゲーム終了, [1] = 階層選択, [2] = やり直し
    public Image selectorImage;

    [Header("表示内容")]
    public Text resultText;
    public Image resultImage;

    [Header("サウンド")]
    public AudioSource successSE;
    public AudioSource moveSE;
    public AudioSource decideSE;

    int selectedOption = 0;
    float inputCooldown = 0.2f;
    float lastInputTime;

    void Start()
    {
        if (successSE != null)
            successSE.Play();

        optionTexts[0].text = "ゲーム終了";
        optionTexts[1].text = "階層選択";
        optionTexts[2].text = "やり直し";

        string stageName = PlayerPrefs.GetString("LastStageName", "不明なステージ");
        resultText.text = $"{stageName}階";

        UpdateSelector();
    }

    void Update()
    {
        if (Time.time - lastInputTime < inputCooldown) return;

        bool left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        bool right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        bool decide = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);

        if (left)
        {
            selectedOption = (selectedOption - 1 + optionTexts.Length) % optionTexts.Length;
            UpdateSelector();
            PlayMoveSE();
            lastInputTime = Time.time;
        }

        if (right)
        {
            selectedOption = (selectedOption + 1) % optionTexts.Length;
            UpdateSelector();
            PlayMoveSE();
            lastInputTime = Time.time;
        }

        if (decide)
        {
            PlayDecideSE();
            switch (selectedOption)
            {
                case 0:
                    EndGame();
                    break;
                case 1:
                    ReturnToStageSelect();
                    break;
                case 2:
                    RetryStage();
                    break;
            }
        }
    }

    void UpdateSelector()
    {
        if (selectorImage != null && optionTexts.Length > selectedOption)
        {
            selectorImage.transform.position = optionTexts[selectedOption].transform.position + new Vector3(-300f, 0f, 0f);
        }

        for (int i = 0; i < optionTexts.Length; i++)
        {
            optionTexts[i].color = (i == selectedOption) ? Color.red : Color.black;
        }
    }

    void EndGame()
    {
        SceneManager.LoadScene("Title");
    }

    void ReturnToStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }

    void RetryStage()
    {
        SceneController.ReturnToPreviousScene();
    }

    void PlayMoveSE()
    {
        if (moveSE != null) moveSE.Play();
    }

    void PlayDecideSE()
    {
        if (decideSE != null) decideSE.Play();
    }
}
