using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemoryUI : MonoBehaviour
{
    [System.Serializable]
    public class StoryImagePair
    {
        public Button button;
        public GameObject storyImage;
        public int requiredScore;
    }

    public StoryImagePair[] storyImagePairs;
    public Button switchSceneButton;
    public string nextSceneName = "Animation-Lv1Mid";
    public Text scoreText;

    private int currentScore = 0;
    private int[] requiredScores = new int[] { 0, 1000, 5000, 15000 };
    private GameObject currentActiveImage;

    private void Start()
    {
        SetRequiredScores();

        // 临时设置分数为50000（仅用于调试）
        PlayNoteModel.score = 50000;


        foreach (var pair in storyImagePairs)
        {
            pair.button.onClick.AddListener(() => ShowStoryImage(pair));
            
            // 为每个故事图片添加点击监听器
            AddClickListener(pair.storyImage);
            
            pair.storyImage.SetActive(false);
        }

        if (switchSceneButton != null)
        {
            switchSceneButton.onClick.AddListener(SwitchScene);
        }
        else
        {
            Debug.LogWarning("Switch Scene Button is not assigned!");
        }

        // 从 PlayNoteModel 获取分数
        int gameScore = PlayNoteModel.score;
        UpdateScore(gameScore);
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
        UpdateButtonStates();
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
            // 隐藏所有图片
            foreach (var p in storyImagePairs)
            {
                p.storyImage.SetActive(false);
            }

            // 显示选中的图片
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

    // 用于测试的方法，可以在 Unity 编辑器中调用
    public void AddScore(int amount)
    {
        UpdateScore(currentScore + amount);
    }

    // 添加一方法在游戏过程中实时更新分数
    private void Update()
    {
        // 实时更新分数显示
        int currentGameScore = PlayNoteModel.score;
        if (currentScore != currentGameScore)
        {
            int scoreDifference = currentGameScore - currentScore;
            Debug.Log($"[MemoryUI] 分数变化检测 - 当前分数: {currentGameScore}, 之前分数: {currentScore}, 差值: {scoreDifference}");
            
            // 根据分数变化判断是哪个关卡通关
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

    // 添加点击监听器到故事图片
    private void AddClickListener(GameObject storyImage)
    {
        // 获取或添加 Button 组件
        Button imageButton = storyImage.GetComponent<Button>();
        if (imageButton == null)
        {
            imageButton = storyImage.AddComponent<Button>();
        }

        // 设置按钮的过渡效果为无
        ColorBlock colors = imageButton.colors;
        colors.fadeDuration = 0;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        imageButton.colors = colors;

        // 添加点击事件
        imageButton.onClick.AddListener(() => HideStoryImage(storyImage));
    }
}
