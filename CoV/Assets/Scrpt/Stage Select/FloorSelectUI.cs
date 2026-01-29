using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Video;

public class FloorSelectUI : MonoBehaviour
{
    enum SelectState
    {
        FloorSelect,
        StartSelect
    }

    SelectState currentState = SelectState.FloorSelect;

    [System.Serializable]
    public class FloorItem
    {
        public GameObject arrow;
        public CanvasGroup arrowCanvasGroup;
        public Text text;

        [Range(1, 5)]
        public int difficultyLevel;

        public VideoClip stageVideo;
        public string sceneName;
    }

    public FloorItem[] floors;

    [Header("フロア表示")]
    public float selectedScale = 1.15f;
    public Color selectedColor = Color.red;
    public Color normalColor = Color.white;

    [Header("矢印フェード")]
    public float arrowFadeSpeed = 2.0f;
    public float arrowMinAlpha = 0.3f;
    public float arrowMaxAlpha = 1.0f;

    [Header("難易度表示")]
    public Text difficultyText;

    [Header("ステージ動画")]
    public VideoPlayer stageVideoPlayer;

    [Header("開始ボタン")]
    public Image startButtonBackground;
    public Text startButtonText;

    [Header("サウンド")]
    public AudioSource decideSE;

    [Header("フェード")]
    public CanvasGroup fadeCanvas;
    public float fadeTime = 0.5f;

    int currentIndex = 0;
    int lastVideoIndex = -1;

    bool isTransitioning = false;

    // ★ 左スティック制御用
    bool stickNeutral = true;

    void Start()
    {
        startButtonBackground.gameObject.SetActive(false);

        stageVideoPlayer.playOnAwake = false;
        stageVideoPlayer.isLooping = true;

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

        float stickY = Input.GetAxisRaw("Vertical");

        // スティックが戻ったら入力解禁
        if (Mathf.Abs(stickY) < 0.3f)
        {
            stickNeutral = true;
        }

        bool up =
            (stickY > 0.5f && stickNeutral) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W);

        bool down =
            (stickY < -0.5f && stickNeutral) ||
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.S);

        bool decide =
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.JoystickButton0);

        if (currentState == SelectState.FloorSelect)
        {
            if (up)
            {
                ChangeSelection(-1);
                stickNeutral = false;
                return;
            }

            if (down)
            {
                ChangeSelection(1);
                stickNeutral = false;
                return;
            }

            if (decide)
            {
                currentState = SelectState.StartSelect;
                startButtonBackground.gameObject.SetActive(true);
                UpdateStartButton(true);
                return;
            }
        }
        else
        {
            if (up || down)
            {
                currentState = SelectState.FloorSelect;
                startButtonBackground.gameObject.SetActive(false);
                UpdateStartButton(false);
                stickNeutral = false;
                return;
            }

            if (decide)
            {
                DecideFloor();
            }
        }
    }

    void LateUpdate()
    {
        if (currentState != SelectState.FloorSelect) return;

        var floor = floors[currentIndex];
        if (floor.arrowCanvasGroup == null) return;

        float alpha = Mathf.PingPong(
            Time.time * arrowFadeSpeed,
            arrowMaxAlpha - arrowMinAlpha
        ) + arrowMinAlpha;

        floor.arrowCanvasGroup.alpha = alpha;
    }

    void ChangeSelection(int dir)
    {
        currentIndex += dir;

        if (currentIndex < 0)
            currentIndex = floors.Length - 1;
        else if (currentIndex >= floors.Length)
            currentIndex = 0;

        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            bool selected = (i == currentIndex);

            floors[i].arrow.SetActive(true);

            if (floors[i].arrowCanvasGroup != null)
            {
                floors[i].arrowCanvasGroup.alpha =
                    selected ? arrowMaxAlpha : 0f;
            }

            floors[i].text.color = selected ? selectedColor : normalColor;
            floors[i].text.transform.localScale =
                selected ? Vector3.one * selectedScale : Vector3.one;
        }

        difficultyText.text =
            GetDifficultyStars(floors[currentIndex].difficultyLevel);

        PlayStageVideo(currentIndex);
    }

    void PlayStageVideo(int index)
    {
        if (index == lastVideoIndex) return;
        if (floors[index].stageVideo == null) return;

        lastVideoIndex = index;

        stageVideoPlayer.Stop();
        stageVideoPlayer.clip = floors[index].stageVideo;

        stageVideoPlayer.Prepare();
        stageVideoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared;
        vp.Play();
    }

    void UpdateStartButton(bool selected)
    {
        startButtonText.color = selected ? Color.red : Color.white;
    }

    string GetDifficultyStars(int level)
    {
        string result = "";
        for (int i = 0; i < 5; i++)
        {
            result += (i < level) ? "★" : "☆";
        }
        return result;
    }

    void DecideFloor()
    {
        if (decideSE != null)
            decideSE.Play();

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