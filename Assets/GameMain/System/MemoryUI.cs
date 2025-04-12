using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MemoryUI : MonoBehaviour
{
    [System.Serializable]
    public class StoryImagePair
    {
        public Button button;//继续按钮
        public GameObject storyImage;
        public int requiredScore;
    }

    public StoryImagePair[] storyImagePairs;
    public Button switchSceneButton;
    public Button scoreboardButton;      // 分数面板按钮
    public GameObject scoreboardImage;   // 分数面板图片
    public string nextSceneName = "Animation-Lv1Mid";
    public Text scoreText;
    public Text scoreboardScoreText;    // 记分板中显示分数的文本
    public Text maxComboText;          // 显示最大连击数的文本
    public Text accuracyText;          // 显示精度的文本
    public Text puzzleSuccessCountText; // 显示解谜成功次数的文本
    public Text puzzleFailCountText;    // 显示解谜失败次数的文本

    public Image rankImage;              // 显示等级图片的UI
    public Sprite[] rankSprites;         // 四个等级对应的图片资源（长度应为4）

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;

    private int currentScore = 0;
    private int[] requiredScores = new int[] { 0, 1000, 5000, 15000 };
    private GameObject currentActiveImage;
    private int totalPuzzles = 1; // 总解谜游戏个数

    private void Start()
    {
        SetRequiredScores();

        foreach (var pair in storyImagePairs)
        {
            pair.button.onClick.AddListener(() => ShowStoryImage(pair));
            AddClickListener(pair.storyImage);
            pair.storyImage.SetActive(false);
        }

        if (switchSceneButton != null)
        {
            switchSceneButton.onClick.AddListener(SwitchScene);
        }

        if (scoreboardButton != null)
        {
            scoreboardButton.onClick.AddListener(ShowScoreboard);
            SetupButtonEffects(scoreboardButton);
        }

        if (scoreboardImage != null)
        {
            scoreboardImage.SetActive(false);
            AddScoreboardClickListener();
        }

        int gameScore = PlayNoteModel.score;
        UpdateScore(gameScore);
    }

    private void SetupButtonEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            PlayHoverSound();
        });
        trigger.triggers.Add(enterEntry);

        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void PlayHoverSound()
    {
        if (audioSource != null && buttonHoverSound != null)
        {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }

    private void ShowScoreboard()
    {
        if (scoreboardImage != null)
        {
            Transform parent = scoreboardImage.transform.parent;
            if (parent != null)
            {
                parent.gameObject.SetActive(true);
            }

            scoreboardImage.SetActive(true);

            if (scoreboardScoreText != null)
            {
                scoreboardScoreText.text = currentScore.ToString();
            }

            if (maxComboText != null)
            {
                maxComboText.text = PlayNoteModel.GetMaxCombo().ToString();
            }

            if (puzzleSuccessCountText != null)
            {
                puzzleSuccessCountText.text = GlobalCounters.PuzzleSuccessCount.ToString();
            }

            if (puzzleFailCountText != null)
            {
                int failCount = totalPuzzles - GlobalCounters.PuzzleSuccessCount;
                puzzleFailCountText.text = failCount.ToString();
            }

            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(false);
            }

            if (scoreboardButton != null)
            {
                scoreboardButton.gameObject.SetActive(false);
            }
            if (switchSceneButton != null)
            {
                switchSceneButton.gameObject.SetActive(false);
            }
        }
    }

    private void HideScoreboard()
    {
        if (scoreboardImage != null)
        {
            Transform parent = scoreboardImage.transform.parent;
            if (parent != null)
            {
                parent.gameObject.SetActive(false);
            }

            scoreboardImage.SetActive(false);

            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true);
            }

            if (scoreboardButton != null)
            {
                scoreboardButton.gameObject.SetActive(true);
            }
            if (switchSceneButton != null)
            {
                switchSceneButton.gameObject.SetActive(true);
            }
        }
    }

    private void AddScoreboardClickListener()
    {
        Button scoreboardButton = scoreboardImage.GetComponent<Button>();
        if (scoreboardButton == null)
        {
            scoreboardButton = scoreboardImage.AddComponent<Button>();
        }

        ColorBlock colors = scoreboardButton.colors;
        colors.fadeDuration = 0;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        scoreboardButton.colors = colors;

        scoreboardButton.onClick.AddListener(HideScoreboard);
    }

    private void SetRequiredScores()
    {
        for (int i = 0; i < storyImagePairs.Length && i < requiredScores.Length; i++)
        {
            storyImagePairs[i].requiredScore = requiredScores[i];
        }
    }

    public void UpdateScore(int newScore)
    {
        currentScore = newScore;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }

        if (scoreboardScoreText != null)
        {
            scoreboardScoreText.text = currentScore.ToString();
        }

        if (accuracyText != null)
        {
            float accuracy = ((float)currentScore / 24400) * 100;
            accuracyText.text = accuracy.ToString("F2") + "%";
        }

        UpdateRankImage(currentScore);
        UpdateButtonStates();
    }

    private void UpdateRankImage(int score)
    {
        if (rankImage != null && rankSprites != null && rankSprites.Length >= 4)
        {
            int rankIndex = 0;
            for (int i = 0; i < requiredScores.Length; i++)
            {
                if (score >= requiredScores[i])
                {
                    rankIndex = i;
                }
            }
            rankImage.sprite = rankSprites[rankIndex];
        }
    }

    private void UpdateButtonStates()
    {
        foreach (var pair in storyImagePairs)
        {
            bool isActive = currentScore >= pair.requiredScore;
            pair.button.interactable = isActive;
            SetButtonHighlight(pair.button, isActive);
        }
    }

    private void SetButtonHighlight(Button button, bool highlight)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlight ? Color.yellow : Color.white;
        button.colors = colors;
    }

    private void ShowStoryImage(StoryImagePair pair)
    {
        if (currentScore >= pair.requiredScore)
        {
            foreach (var p in storyImagePairs)
            {
                p.storyImage.SetActive(false);
            }

            pair.storyImage.SetActive(true);
            currentActiveImage = pair.storyImage;
            Debug.Log($"Showing StoryImage for button: {pair.button.name}");
        }
    }

    private void HideStoryImage(GameObject storyImage)
    {
        storyImage.SetActive(false);
        currentActiveImage = null;
        Debug.Log("Hiding StoryImage");
    }

    private void SwitchScene()
    {
        Debug.Log($"Switching to scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }

    public void AddScore(int amount)
    {
        UpdateScore(currentScore + amount);
    }

    private void Update()
    {
        int currentGameScore = PlayNoteModel.score;
        if (currentScore != currentGameScore)
        {
            int scoreDifference = currentGameScore - currentScore;
            Debug.Log($"[MemoryUI] 分数变化检测 - 当前分数: {currentGameScore}, 之前分数: {currentScore}, 差值: {scoreDifference}");

            if (scoreDifference == 10000)
            {
                Debug.Log("[MemoryUI] Puzzle2 通关奖励: +10000分");
            }
            else if (scoreDifference == 2000)
            {
                Debug.Log("[MemoryUI] Puzzle3 通关奖励: +2000分");
            }
            else
            {
                Debug.Log($"[MemoryUI] 其他分数变化: +{scoreDifference}分");
            }

            UpdateScore(currentGameScore);
        }
    }

    private void AddClickListener(GameObject storyImage)
    {
        Button imageButton = storyImage.GetComponent<Button>();
        if (imageButton == null)
        {
            imageButton = storyImage.AddComponent<Button>();
        }

        ColorBlock colors = imageButton.colors;
        colors.fadeDuration = 0;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        imageButton.colors = colors;

        imageButton.onClick.AddListener(() => HideStoryImage(storyImage));
    }
}
