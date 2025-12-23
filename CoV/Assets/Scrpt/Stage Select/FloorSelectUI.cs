using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FloorSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class FloorItem
    {
        public GameObject arrow;
        public Text text;

        [Range(1, 5)]
        public int difficultyLevel;   // ★の数（1〜5）

        public Sprite stageSprite;
        public string sceneName;
    }

    public FloorItem[] floors;

    [Header("表示")]
    public float selectedScale = 1.15f;
    public Color selectedColor = Color.white;
    public Color normalColor = Color.gray;

    [Header("難易度・画像表示")]
    public Text difficultyText;
    public Image stageImage;

    [Header("フェード")]
    public CanvasGroup fadeCanvas;
    public float fadeTime = 0.5f;

    int currentIndex = 0;
    float inputCooldown = 0.2f;
    float lastInputTime;
    bool isTransitioning = false;

    void Start()
    {
        UpdateUI();

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            StartCoroutine(FadeIn());
        }
    }

    void Update()
    {
        if (isTransitioning) return;
        if (floors.Length == 0) return;
        if (Time.time - lastInputTime < inputCooldown) return;

        // コントローラー 十字キー上下
        float dpadY = Input.GetAxisRaw("DPadX");

        if (dpadY > 0.5f)
        {
            ChangeSelection(-1); // 上
            return;
        }
        else if (dpadY < -0.5f)
        {
            ChangeSelection(1);  // 下
            return;
        }

        // キーボード 上
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ChangeSelection(-1);
            return;
        }

        // キーボード 下
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ChangeSelection(1);
            return;
        }

        // 決定
        if (
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.JoystickButton0)
        )
        {
            DecideFloor();
        }
    }

    void ChangeSelection(int dir)
    {
        currentIndex += dir;

        if (currentIndex < 0)
            currentIndex = floors.Length - 1;
        else if (currentIndex >= floors.Length)
            currentIndex = 0;

        lastInputTime = Time.time;
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            bool selected = (i == currentIndex);

            floors[i].arrow.SetActive(selected);
            floors[i].text.color = selected ? selectedColor : normalColor;
            floors[i].text.transform.localScale =
                selected ? Vector3.one * selectedScale : Vector3.one;
        }

        difficultyText.text =
            GetDifficultyStars(floors[currentIndex].difficultyLevel);

        stageImage.sprite = floors[currentIndex].stageSprite;
    }

    string GetDifficultyStars(int level)
    {
        int maxStars = 5;
        string result = "";

        for (int i = 0; i < maxStars; i++)
        {
            result += (i < level) ? "★" : "☆";
        }

        return result;
    }

    void DecideFloor()
    {
        isTransitioning = true;
        StartCoroutine(FadeOutAndLoad(floors[currentIndex].sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
    }
}